using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Mapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class CaseOpenningManager : ICaseOpenningRepository
    {
        protected readonly CSGOATDbContext _context;
        protected readonly IMapper _mapper;
        protected readonly IReadableRepository<Case, int> _caseRepository;
        protected readonly IFairRandomRepository _fairRandomRepository;
        protected readonly IDataRepository<InventoryItem, int> _inventoryItemRepository;
        protected readonly IPromoCodeRepository _promoCodeRepository;
        protected readonly IDataRepository<RandomTransaction, int> _randomTransactionRepository;
        protected readonly IUserRepository _userRepository;

        public CaseOpenningManager(
            CSGOATDbContext context,
            IMapper mapper,
            IReadableRepository<Case, int> caseRepository,
            IFairRandomRepository fairRandomRepository,
            IDataRepository<InventoryItem, int> inventoryItemRepository,
            IPromoCodeRepository promoCodeRepository,
            IDataRepository<RandomTransaction, int> randomTransactionRepository,
            IUserRepository userRepository
            )
        {
            _context = context;
            _mapper = mapper;
            _caseRepository = caseRepository;
            _fairRandomRepository = fairRandomRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _promoCodeRepository = promoCodeRepository;
            _randomTransactionRepository = randomTransactionRepository;
            _userRepository = userRepository;
        }

        public async Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            int userId)
        {
            User? user = await _userRepository.GetByIdAsync(userId, "FairRandom");
            if (user == null) throw new Exception("User not found.");
            return await OpenCaseAsync(caseOpenningDTO, user);
        }

        public async Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            User user)
        {
            if (caseOpenningDTO.Quantity <= 0) throw new Exception("Quantity must be greater than zero.");
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();


            QueryOptions<Case> options = new QueryOptions<Case>()
                .Before("CaseContents.Skin.Rarity",
                        "CaseContents.Skin.Item.ItemType",
                        "CaseContents.Skin.Wears.WearType")
                .After("CaseContents.Skin.PriceHistories");
            Case? targetCase = await _caseRepository.GetByIdAsync(
                caseOpenningDTO.CaseId,
                options
            );
            if (targetCase == null) throw new Exception("Case not found.");

            PromoCode? promoCode = null;
            if (!string.IsNullOrEmpty(caseOpenningDTO.PromoCode))
            {
                promoCode = await _promoCodeRepository.Check(
                    caseOpenningDTO.PromoCode!,
                    user.UserId,
                    caseOpenningDTO.CaseId
                );
                if (promoCode == null)
                    throw new Exception("Promo code is invalid.");
            }

            double basePrice = targetCase.CasePrice * caseOpenningDTO.Quantity;
            double finalPrice = promoCode != null
                ? promoCode.Apply(basePrice)
                : basePrice;
            CasePromoCodeDTO? promoCodeDTO = null;
            if (promoCode != null)
            {
                promoCodeDTO = _mapper.Map<CasePromoCodeDTO>(promoCode);
                promoCodeDTO.BasePrice = basePrice;
                promoCodeDTO.FinalPrice = finalPrice;
                await _promoCodeRepository.Consume(promoCode);
            }

            MultipleCaseResultDTO finalResult = new MultipleCaseResultDTO
            {
                CaseId = targetCase.CaseId,
                WalletBefore = user.Wallet,
                WalletAfter = user.Wallet,
                ContentLength = 0,
                PromoCode = promoCodeDTO
            };

            // TODO: Verify the user is not currently banned from opening cases
            // set finalResult.Ban if relevant
            // TODO: Verify the user would not exceed their spending limits by opening this case
            // set finalResult.Limit if relevant
            if (
                finalPrice > user.Wallet
                || finalResult.Ban != null
                || finalResult.Limit != null
            ) return finalResult;

            user.Wallet -= finalPrice;
            await _userRepository.UpdateAsync(user);
            finalResult.WalletAfter = user.Wallet;
            finalResult.PromoCode = promoCodeDTO;

            FairRandom initRandom = await _fairRandomRepository.Init(user, true);
            for (int i = 0; i < caseOpenningDTO.Quantity; i++)
            {
                FairRandom fairRandom = await _fairRandomRepository.Resolve(
                    user, initRandom
                );
                float floatValue = (float)fairRandom.Fraction2!;
                Skin skin = ChooseOne(targetCase.CaseContents, (double)fairRandom.Fraction1!).Skin;
                Wear wear = skin.GetClosestWear(floatValue);
                InventoryItem newItem = new InventoryItem
                {
                    UserId = user.UserId,
                    WearId = wear.WearId,
                    Float = floatValue,
                };
                await _inventoryItemRepository.AddAsync(newItem);

                RandomTransaction randomTransaction = new RandomTransaction
                {
                    UserId = user.UserId,
                    WalletValue = -finalPrice / caseOpenningDTO.Quantity,
                    InventoryItemId = newItem.InventoryItemId,
                    CaseId = targetCase.CaseId,
                    FairRandomId = fairRandom.FairRandomId,
                };
                await _randomTransactionRepository.AddAsync(randomTransaction);

                IndividualCaseResultDTO indivResult = new IndividualCaseResultDTO
                {
                    Reward = _mapper.Map<InventoryItemDetailDTO>(newItem),
                    RandomTransactionId = randomTransaction.TransactionId,
                    Random = _mapper.Map<FairRandomDTO>(fairRandom),
                    Roller = CreateRoller(targetCase.CaseContents, caseOpenningDTO.RaffleRollerLength)
                };

                finalResult.Results.Add(indivResult);
                finalResult.ContentLength ++;
            }

            foreach (CaseContent cc in targetCase.CaseContents)
            {
                finalResult.Skins[cc.SkinId] = _mapper.Map<SkinDTO>(cc.Skin);
            }

            await transaction.CommitAsync();
            return finalResult;
        }

        public static CaseContent ChooseOne(IEnumerable<CaseContent> options, double fraction)
        {
            int totalWeight = options.Sum(o => o.Weight);
            double scaled = fraction * totalWeight;
            int cumulative = 0;
            foreach (CaseContent option in options)
            {
                cumulative += option.Weight;
                if (scaled < cumulative)
                    return option;
            }
            return options.Last();
        }

        public static int[] CreateRoller(IEnumerable<CaseContent> options, int length)
        {
            int[] roller = new int[length];
            Random rand = new();
            for (int i = 0; i < length; i++)
            {
                double fraction = rand.NextDouble();
                CaseContent chosen = ChooseOne(options, fraction);
                roller[i] = chosen.SkinId;
            }
            return roller;
        }
    }
}