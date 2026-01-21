using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Manages item upgrade/downgrade operations with probability calculations based on prices and wear
    /// </summary>
    /// <remarks>
    /// Implements complex probability formulas that determine success rates when users combine items,
    /// potentially resulting in upgraded, downgraded, or destroyed outcomes.
    /// </remarks>
    public class UpgradeManager : IUpgradeRepository
    {
        private readonly CSGOATDbContext _context;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IReadableRepository<Skin, int> _skinRepository;
        private readonly IDataRepository<InventoryItem, int> _inventoryItemRepository;
        private readonly IFairRandomRepository _fairRandomRepository;
        private readonly IDataRepository<UpgradeResult, (int, int)> _upgradeResultRepository;

        public UpgradeManager(
            CSGOATDbContext context,
            IMapper mapper,
            IUserRepository userRepository,
            IReadableRepository<Skin, int> skinRepository,
            IDataRepository<InventoryItem, int> inventoryItemRepository,
            IFairRandomRepository fairRandomRepository,
            IDataRepository<UpgradeResult, (int, int)> upgradeResultRepository
            )
        {
            _context = context;
            _mapper = mapper;
            _userRepository = userRepository;
            _skinRepository = skinRepository;
            _inventoryItemRepository = inventoryItemRepository;
            _fairRandomRepository = fairRandomRepository;
            _upgradeResultRepository = upgradeResultRepository;
        }

        private float Degrade(float original, double random, string function)
        {
            switch (function.ToLower())
            {
                case "none":
                    return original;
                case "uniform":
                    return (float)random * original;
                default:
                    throw new Exception($"Unknown degrade function {function}.");
            }
        }

        public class PriceInfo
        {
            private IEnumerable<double> Prices { get; set; } = null!;
            public double TotalPrice => Prices.Sum();
            public double AveragePrice => Prices.Average();
            public double MinPrice => Prices.Min();
            public double MaxPrice => Prices.Max();
            public int PriceCount => Prices.Count();

            public PriceInfo(IEnumerable<double?> prices)
            {
                Prices = prices.Where(p => p != null).Select(p => p!.Value);
            }

            public PriceInfo(IEnumerable<double> prices)
            {
                Prices = prices;
            }
        }

        private double SkinUpgrade(
            PriceInfo invItemPrice,
            PriceInfo targetSkinPrice,
            double monetaryValue)
        {
            double p = targetSkinPrice.AveragePrice;
            double t = invItemPrice.TotalPrice + monetaryValue;
            // \frac{p^{2}}{\left(t+1\right)^{2}+p^{2}-1}
            double probDestroy = Math.Max(0.0, Math.Min(1.0,
                Math.Pow(p, 2) / (Math.Pow(t + 1, 2) + Math.Pow(p, 1.75) - 1)
            ));
            return probDestroy;
        }

        private UpgradeResultDTO ItemDowngrade(
            double successProbability,
            InventoryItem item,
            PriceInfo invItemPrice,
            double monetaryValue)
        {
            double m = invItemPrice.AveragePrice;
            double t = invItemPrice.TotalPrice + monetaryValue;
            double z = monetaryValue;
            double? i = item.Wear.CurrentPrice;
            if (i == null)
            {
                return new UpgradeResultDTO()
                {
                    FloatStart = item.Float,
                    ProbIntact = 1,
                    ProbDegrade = 0,
                    PropDestroy = 0,
                    DegradeFunction = "None"
                };
            }
            // \left(1-\frac{m}{t}\right)\left(1-\frac{i}{t}\right)\left(1-\frac{m}{i+z}\right)
            double keepProb = Math.Max(0.0, Math.Min(1.0,
                (1 - (m / t)) *
                (1 - (i.Value / t)) *
                (1 - (m / (i.Value + z)))
            ));
            return new UpgradeResultDTO
            {
                FloatStart = item.Float,
                ProbIntact = keepProb * (1 - successProbability),
                ProbDegrade = keepProb * successProbability,
                PropDestroy = 1 - keepProb,
                DegradeFunction = "Uniform"
            };
        }


        public async Task<UpgradeOutputDTO> UpgradeAsync(UpgradeInputDTO dto, int userId)
        {
            QueryOptions<User> options1 = new QueryOptions<User>()
                .Before(u => u.FairRandom);
            User? user = await _userRepository.GetByIdAsync(userId, options1);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            QueryOptions<Skin> option2 = new QueryOptions<Skin>()
                .Before(s => s.Rarity, s => s.Item, s => s.Wears);
            Skin? skin = await _skinRepository.GetByIdAsync(dto.TargetSkinId, option2);
            if (skin == null)
            {
                throw new Exception("Skin not found.");
            }

            IEnumerable<PriceHistory> priceHistories = _context.PriceHistories
                .Where(ph => ph.SkinId == skin.SkinId).OrderByDescending(ph => ph.PriceDate).Take(skin.Wears.Count);
            var skinPrice = new PriceInfo(priceHistories.Select(ph => ph.PriceValue));
            if (skinPrice.PriceCount == 0)
            {
                throw new Exception("Target skin has no price history.");
            }

            QueryOptions<InventoryItem> options2 = new QueryOptions<InventoryItem>()
                .Before(i => i.Wear.Skin.Rarity)
                .Before(i => dto.InventoryItemIds.Contains(i.InventoryItemId))
                .Before(i => i.UserId == user.UserId)
                .Before(i => i.RemovedOn == null)
                .After(i => i.Wear.WearClass.PriceHistories);
            IEnumerable<InventoryItem> invItems = await _inventoryItemRepository.GetAllAsync(options2);
            if (invItems.Count() != dto.InventoryItemIds.Count)
            {
                throw new Exception("One or more inventory items not found.");
            }

            var invItemPrice = new PriceInfo(invItems.Select(i => i.Wear.CurrentPrice));

            UpgradeOutputDTO output = new()
            {
                Preview = dto.Preview,
                FailProbability = SkinUpgrade(
                    invItemPrice,
                    skinPrice,
                    dto.MonetaryValue
                )
            };

            foreach (InventoryItem inventoryItem in invItems)
            {
                UpgradeResultDTO itemUpgradeResult = ItemDowngrade(
                    output.FailProbability,
                    inventoryItem,
                    invItemPrice,
                    dto.MonetaryValue
                );
                InventoryItemDTO itemDTO = _mapper.Map<InventoryItemDTO>(inventoryItem);
                output.Items.Add(new UpgradeOutputItemDTO()
                {
                    InventoryItem = itemDTO,
                    UpgradeResult = itemUpgradeResult
                });
            }
            if (!dto.Preview)
            {
                output = await ExecuteUpgradeAsync(
                    output, user, skin, invItems, dto.MonetaryValue);
            }
            return output;
        }


        private async Task<UpgradeOutputItemDTO> Resolve(
            UpgradeOutputItemDTO tuple,
            IEnumerable<InventoryItem> invItems,
            RandomTransaction randomTransaction,
            FairRandom fairRandom, User user)
        {
            FairRandom newRandom = await _fairRandomRepository.Resolve(user, fairRandom, true);
            InventoryItem invItem = invItems
                .First(i => i.InventoryItemId == tuple.InventoryItem.InventoryItemId);

            // If item is not left intact
            if (tuple.UpgradeResult.ProbIntact < 1 && newRandom.Fraction1 < (1 - tuple.UpgradeResult.ProbIntact))
            {
                if (newRandom.Fraction1 < tuple.UpgradeResult.PropDestroy)
                {
                    // Destroy item
                    invItem.RemovedOn = DateTime.UtcNow;
                    tuple.UpgradeResult.FloatEnd = 0;
                }
                else
                {
                    // Degrade item
                    float newFloat = Degrade(
                        (float)tuple.UpgradeResult.FloatStart!,
                        (double)newRandom.Fraction2!,
                        tuple.UpgradeResult.DegradeFunction
                    );
                    invItem.Float = newFloat;
                    tuple.UpgradeResult.FloatEnd = newFloat;
                    Wear targetWear = invItem.Wear.Skin.GetClosestWear(newFloat);
                    invItem.WearId = targetWear.WearId;
                }
                await _inventoryItemRepository.UpdateAsync(invItem);
                QueryOptions<InventoryItem> options = new QueryOptions<InventoryItem>()
                    .Before(i => i.Wear.Skin.Rarity);
                invItem = (await _inventoryItemRepository.GetByIdAsync(invItem.InventoryItemId, options))!;
                tuple.InventoryItem = _mapper.Map<InventoryItemDTO>(invItem);
            }
            else
            {
                // Item stays intact
                tuple.UpgradeResult.FloatEnd = invItem.Float;
            }

            var upgradeResult = new UpgradeResult()
            {
                InventoryItemId = invItem.InventoryItemId,
                TransactionId = randomTransaction.TransactionId,
                FairRandomId = newRandom.FairRandomId,
                FloatStart = (float)tuple.UpgradeResult.FloatStart!,
                FloatEnd = tuple.UpgradeResult.FloatEnd ?? invItem.Float,
                ProbIntact = tuple.UpgradeResult.ProbIntact,
                ProbDegrade = tuple.UpgradeResult.ProbDegrade,
                PropDestroy = tuple.UpgradeResult.PropDestroy,
                DegradeFunction = tuple.UpgradeResult.DegradeFunction
            };
            _ = await _upgradeResultRepository.AddAsync(upgradeResult);
            tuple.UpgradeResult = _mapper.Map<UpgradeResultDTO>(upgradeResult);
            await _context.Entry(upgradeResult).Reference(u => u.FairRandom).LoadAsync();
            tuple.UpgradeResult.FairRandom = _mapper.Map<FairRandomDTO>(upgradeResult.FairRandom);
            return tuple;
        }


        private async Task<UpgradeOutputDTO> ExecuteUpgradeAsync(UpgradeOutputDTO dto,
            User user, Skin skin, IEnumerable<InventoryItem> invItems, double monetaryValue)
        {
            if (user.Wallet < monetaryValue)
            {
                throw new Exception("Insufficient funds.");
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            FairRandom initRandom = await _fairRandomRepository.Resolve(user, null, true);
            dto.FairRandom = _mapper.Map<FairRandomDTO>(initRandom);
            InventoryItem? newItem = null;

            if (initRandom.Fraction1 > dto.FailProbability)
            {
                Wear targetWear = skin.GetClosestWear((float)initRandom.Fraction2!);
                newItem = new InventoryItem()
                {
                    UserId = user.UserId,
                    WearId = targetWear.WearId,
                    Float = (float)initRandom.Fraction2!,
                    IsFavorite = false
                };
                _ = await _inventoryItemRepository.AddAsync(newItem);

                QueryOptions<InventoryItem> options = new QueryOptions<InventoryItem>()
                    .Before(i => i.Wear.Skin.Rarity, i => i.Wear.WearType,
                    i => i.Wear.Skin.Item.ItemType)
                    .After(i => i.Wear.WearClass.PriceHistories);
                newItem = await _inventoryItemRepository.GetByIdAsync(newItem.InventoryItemId, options);
                dto.ItemResult = _mapper.Map<InventoryItemDetailDTO>(newItem);
            }

            var randomTransaction = new RandomTransaction()
            {
                UserId = user.UserId,
                FairRandomId = initRandom.FairRandomId,
                CaseId = null,
                WalletValue = -monetaryValue,
                InventoryItemId = newItem?.InventoryItemId
            };
            _ = _context.ItemTransactions.Add(randomTransaction);

            List<UpgradeOutputItemDTO> resolvedItems = [];
            foreach (UpgradeOutputItemDTO item in dto.Items)
            {
                UpgradeOutputItemDTO resolved = await Resolve(item, invItems, randomTransaction, initRandom, user);
                resolvedItems.Add(resolved);
            }
            dto.Items = resolvedItems;

            user.Wallet -= monetaryValue;
            await _userRepository.UpdateAsync(user);
            _ = await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return dto;
        }
    }
}
