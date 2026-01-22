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

        /// <summary>
        /// Applies degradation to an item's float value based on the degradation function
        /// </summary>
        /// <param name="original">The item's current float value</param>
        /// <param name="random">The random value from provably fair system (0.0-1.0)</param>
        /// <param name="function">The degradation function type ("none" or "uniform")</param>
        /// <returns>The degraded float value</returns>
        /// <remarks>
        /// Degradation functions:
        /// - "none": Returns original unchanged
        /// - "uniform": Multiplies original by random value (uniform distribution degradation)
        /// </remarks>
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

        /// <summary>
        /// Helper class for aggregating price statistics used in upgrade probability calculations
        /// </summary>
        /// <remarks>
        /// Provides statistical analysis of item prices:
        /// - TotalPrice: Sum of all prices (used for t parameter in formulas)
        /// - AveragePrice: Mean price (used for m or p parameters)
        /// - MinPrice: Lowest price in collection
        /// - MaxPrice: Highest price in collection
        /// - PriceCount: Number of prices
        /// 
        /// Supports initialization from nullable double collection (filters nulls) or regular doubles
        /// </remarks>
        public class PriceInfo
        {
            private IEnumerable<double> Prices { get; set; } = null!;
            public double TotalPrice => Prices.Sum();
            public double AveragePrice => Prices.Average();
            public double MinPrice => Prices.Min();
            public double MaxPrice => Prices.Max();
            public int PriceCount => Prices.Count();

            /// <summary>
            /// Initialize from nullable prices, filtering out null values
            /// </summary>
            public PriceInfo(IEnumerable<double?> prices)
            {
                Prices = prices.Where(p => p != null).Select(p => p!.Value);
            }

            /// <summary>
            /// Initialize from non-null prices
            /// </summary>
            public PriceInfo(IEnumerable<double> prices)
            {
                Prices = prices;
            }
        }

        /// <summary>
        /// Calculates the failure probability for an item upgrade based on the mathematical formula
        /// </summary>
        /// <param name="invItemPrice">Price statistics of inventory items being combined</param>
        /// <param name="targetSkinPrice">Price statistics of target skin</param>
        /// <param name="monetaryValue">User-provided wallet credit for the upgrade</param>
        /// <returns>Probability value between 0.0 and 1.0 representing failure chance</returns>
        /// <remarks>
        /// Uses formula: p^2 / ((t+1)^2 + p^1.75 - 1) where
        /// p = average target skin price
        /// t = total inventory items price + monetary value
        /// Result clamped between 0.0 and 1.0 for valid probability range
        /// </remarks>
        private double SkinUpgrade(
            PriceInfo invItemPrice,
            PriceInfo targetSkinPrice,
            double monetaryValue)
        {
            double p = targetSkinPrice.AveragePrice;
            double t = invItemPrice.TotalPrice + monetaryValue;
            // Formula: p^2 / ((t+1)^2 + p^1.75 - 1)
            // Desmos: \frac{p^{2}}{\left(t+1\right)^{2}+p^{1.75}-1}
            // Probability of upgrade failure based on price ratios
            double probDestroy = Math.Max(0.0, Math.Min(1.0,
                Math.Pow(p, 2) / (Math.Pow(t + 1, 2) + Math.Pow(p, 1.75) - 1)
            ));
            return probDestroy;
        }

        /// <summary>
        /// Calculates individual item downgrade probabilities within an upgrade operation
        /// </summary>
        /// <param name="successProbability">The overall upgrade failure probability</param>
        /// <param name="item">The inventory item being evaluated</param>
        /// <param name="invItemPrice">Price statistics of all inventory items</param>
        /// <param name="monetaryValue">User-provided wallet credit</param>
        /// <returns>UpgradeResultDTO with probability breakdown</returns>
        /// <remarks>
        /// Calculates three probabilities:
        /// - ProbIntact: Chance item remains unchanged = keepProb * (1 - successProbability)
        /// - ProbDegrade: Chance item is degraded = keepProb * successProbability
        /// - PropDestroy: Chance item is destroyed = 1 - keepProb
        /// 
        /// Formula for keepProb: (1 - m/t) * (1 - i/t) * (1 - m/(i+z)) where
        /// m = average inventory item price
        /// t = total inventory items price + monetary value
        /// i = item's current price (or null if no history)
        /// z = monetary value
        /// </remarks>
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
                // No price history - item stays intact with 100% probability
                return new UpgradeResultDTO()
                {
                    FloatStart = item.Float,
                    ProbIntact = 1,
                    ProbDegrade = 0,
                    PropDestroy = 0,
                    DegradeFunction = "None"
                };
            }
            // Formula: (1 - m/t) * (1 - i/t) * (1 - m/(i+z))
            // Desmos: \left(1-\frac{m}{t}\right)\left(1-\frac{i}{t}\right)\left(1-\frac{m}{i+z}\right)
            // Probability that item survives the upgrade intact or degraded
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


        /// <summary>
        /// Initiates an item upgrade operation, optionally previewing or executing the transaction
        /// </summary>
        /// <param name="dto">Upgrade parameters including target skin, inventory items, and monetary value</param>
        /// <param name="userId">The user performing the upgrade</param>
        /// <returns>UpgradeOutputDTO with calculated probabilities and results</returns>
        /// <exception cref="Exception">Thrown if user/skin not found or insufficient wallet balance</exception>
        /// <remarks>
        /// Two modes:
        /// - Preview (dto.Preview=true): Calculates probabilities without modifying database
        /// - Execute (dto.Preview=false): Performs transaction with provably fair randomization
        /// 
        /// Execution flow:
        /// 1. Validates user and target skin exist
        /// 2. Loads price history for probability calculations
        /// 3. Calculates overall upgrade success probability
        /// 4. Calculates individual item outcome probabilities
        /// 5. If executing: resolves random outcomes and modifies inventory
        /// </remarks>
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


        /// <summary>
        /// Resolves a single inventory item's upgrade outcome using provably fair randomization
        /// </summary>
        /// <param name="tuple">The item with probability calculations</param>
        /// <param name="invItems">All inventory items in the upgrade</param>
        /// <param name="randomTransaction">The transaction record for audit trail</param>
        /// <param name="fairRandom">The initial fair random seed</param>
        /// <param name="user">The user performing the upgrade</param>
        /// <returns>Updated UpgradeOutputItemDTO with final outcome</returns>
        /// <remarks>
        /// Resolves three possible outcomes:
        /// 1. Item stays intact (no changes)
        /// 2. Item is degraded (float value changed, wear class updated)
        /// 3. Item is destroyed (RemovedOn timestamp set)
        /// 
        /// Uses provably fair random fractions:
        /// - Fraction1: Determines if item changes (destroy/degrade vs intact)
        /// - Fraction2: Determines degradation amount (if applicable)
        /// </remarks>
        private async Task<UpgradeOutputItemDTO> Resolve(
            UpgradeOutputItemDTO tuple,
            IEnumerable<InventoryItem> invItems,
            RandomTransaction randomTransaction,
            FairRandom fairRandom, User user)
        {
            FairRandom newRandom = await _fairRandomRepository.Resolve(user, fairRandom, true);
            InventoryItem invItem = invItems
                .First(i => i.InventoryItemId == tuple.InventoryItem.InventoryItemId);

            // Determine item outcome: intact (within ProbIntact), degrade, or destroy
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


        /// <summary>
        /// Executes the item upgrade transaction with database persistence and provably fair randomization
        /// </summary>
        /// <param name="dto">The upgrade output with probabilities calculated</param>
        /// <param name="user">The user performing the upgrade</param>
        /// <param name="skin">The target skin for successful upgrade</param>
        /// <param name="invItems">All inventory items being combined</param>
        /// <param name="monetaryValue">Wallet credit being used</param>
        /// <returns>Updated UpgradeOutputDTO with final outcomes and fair random reference</returns>
        /// <remarks>
        /// Transaction flow:
        /// 1. Validates wallet sufficient funds
        /// 2. Begins database transaction for atomicity
        /// 3. Initializes provably fair random session
        /// 4. Creates new item if upgrade succeeds (Fraction1 > FailProbability)
        /// 5. Creates RandomTransaction record for wallet debit
        /// 6. Resolves each item's individual outcome
        /// 7. Deducts from user wallet
        /// 8. Commits all changes atomically
        /// </remarks>
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

            // Check if upgrade succeeds based on initial random fraction
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
