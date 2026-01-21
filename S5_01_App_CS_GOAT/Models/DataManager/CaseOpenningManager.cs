using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Manages the opening of cases with randomized item selection and promo code application
    /// </summary>
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

        /// <summary>
        /// Opens cases for a user specified by ID, applying promo codes and randomized item selection
        /// </summary>
        /// <param name="caseOpenningDTO">The case opening parameters (case ID, quantity, promo code, roller length)</param>
        /// <param name="userId">The ID of the user opening the case</param>
        /// <returns>Result containing opened items, fair random data, and wallet changes</returns>
        /// <remarks>
        /// This method loads the user with their fair random data and delegates to the User-based overload.
        /// </remarks>
        public async Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            int userId)
        {
            QueryOptions<User> options = new QueryOptions<User>()
                .Before(u => u.FairRandom);
            User? user = await _userRepository.GetByIdAsync(userId, options);
            return user == null ? throw new Exception("User not found.") : await OpenCaseAsync(caseOpenningDTO, user);
        }

        /// <summary>
        /// Opens cases for a user, with full transaction handling and randomized item selection
        /// </summary>
        /// <param name="caseOpenningDTO">The case opening parameters</param>
        /// <param name="user">The user opening the case</param>
        /// <returns>Result containing items won, fair random proof, and updated wallet balance</returns>
        /// <remarks>
        /// This method:
        /// 1. Validates quantity is positive
        /// 2. Loads the case with all necessary relationships
        /// 3. Validates and applies promo code if provided
        /// 4. Charges the user's wallet
        /// 5. Generates randomized items using provably fair algorithm
        /// 6. Creates transaction records
        /// Uses database transaction to ensure atomicity.
        /// </remarks>
        public async Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            User user)
        {
            if (caseOpenningDTO.Quantity <= 0)
            {
                throw new Exception("Quantity must be greater than zero.");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            QueryOptions<Case> options = new QueryOptions<Case>()
                .Before("CaseContents.Skin.Rarity",
                        "CaseContents.Skin.Item.ItemType",
                        "CaseContents.Skin.WearClasses.Wears.WearType");
            Case? targetCase = await _caseRepository.GetByIdAsync(
                caseOpenningDTO.CaseId,
                options
            );
            if (targetCase == null)
            {
                throw new Exception("Case not found.");
            }

            PromoCode? promoCode = null;
            if (!string.IsNullOrEmpty(caseOpenningDTO.PromoCode))
            {
                promoCode = await _promoCodeRepository.Check(
                    caseOpenningDTO.PromoCode!,
                    user.UserId,
                    caseOpenningDTO.CaseId
                );
                if (promoCode == null)
                {
                    throw new Exception("Promo code is invalid.");
                }
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

            var finalResult = new MultipleCaseResultDTO
            {
                CaseId = targetCase.CaseId,
                WalletBefore = user.Wallet,
                WalletAfter = user.Wallet,
                ContentLength = 0,
                PromoCode = promoCodeDTO
            };

            if (
                finalPrice > user.Wallet
                || finalResult.Ban != null
                || finalResult.Limit != null
            )
            {
                return finalResult;
            }

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
                var newItem = new InventoryItem
                {
                    UserId = user.UserId,
                    WearId = wear.WearId,
                    Float = floatValue,
                };
                _ = await _inventoryItemRepository.AddAsync(newItem);
                await _context.Entry(newItem.Wear.WearClass).Collection(wc => wc.PriceHistories).LoadAsync();

                var randomTransaction = new RandomTransaction
                {
                    UserId = user.UserId,
                    WalletValue = -finalPrice / caseOpenningDTO.Quantity,
                    InventoryItemId = newItem.InventoryItemId,
                    CaseId = targetCase.CaseId,
                    FairRandomId = fairRandom.FairRandomId,
                };
                _ = await _randomTransactionRepository.AddAsync(randomTransaction);

                var indivResult = new IndividualCaseResultDTO
                {
                    Reward = _mapper.Map<InventoryItemDetailDTO>(newItem),
                    RandomTransactionId = randomTransaction.TransactionId,
                    Random = _mapper.Map<FairRandomDTO>(fairRandom),
                    Roller = CreateRoller(targetCase.CaseContents, caseOpenningDTO.RaffleRollerLength)
                };

                finalResult.Results.Add(indivResult);
                finalResult.ContentLength++;
            }

            foreach (CaseContent cc in targetCase.CaseContents)
            {
                finalResult.Skins[cc.SkinId] = _mapper.Map<SkinDTO>(cc.Skin);
            }

            await transaction.CommitAsync();
            return finalResult;
        }

        /// <summary>
        /// Selects a single case content option based on weighted probabilities and a fractional value
        /// </summary>
        /// <param name="options">The collection of case contents with their weights</param>
        /// <param name="fraction">A normalized value between 0 and 1 (typically from fair random)</param>
        /// <returns>The selected CaseContent based on weighted probability distribution</returns>
        /// <remarks>
        /// This implements weighted random selection by:
        /// 1. Calculating total weight
        /// 2. Scaling the fraction to the total weight
        /// 3. Walking through cumulative weights to find the matching item
        /// </remarks>
        public static CaseContent ChooseOne(IEnumerable<CaseContent> options, double fraction)
        {
            int totalWeight = options.Sum(o => o.Weight);
            double scaled = fraction * totalWeight;
            int cumulative = 0;
            foreach (CaseContent option in options.OrderBy(cc => cc.Weight))
            {
                cumulative += option.Weight;
                if (scaled < cumulative)
                {
                    return option;
                }
            }
            return options.Last();
        }

        /// <summary>
        /// Generates a visual roller array for the case opening animation
        /// </summary>
        /// <param name="options">The collection of case contents to use for the roller</param>
        /// <param name="length">The number of items to include in the roller</param>
        /// <returns>An array of skin IDs representing the animation sequence</returns>
        /// <remarks>
        /// This creates a random sequence of items that are displayed during the case opening animation.
        /// Each item is selected using weighted probability.
        /// </remarks>
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