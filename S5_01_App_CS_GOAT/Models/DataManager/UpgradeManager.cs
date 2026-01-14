using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;
using Stripe;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
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
                case "none": return original;
                case "uniform": return (float)random * original;
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

        private UpgradeResultDTO SkinUpgrade(
            PriceInfo invItemPrice,
            PriceInfo targetSkinPrice,
            double monetaryValue)
        {
            double p = targetSkinPrice.AveragePrice;
            double t = invItemPrice.TotalPrice + monetaryValue;
            // \left(\frac{1}{\left(t+1\right)^{2}+p^{2}-1}p^{2}\right)
            double probDestroy = Math.Max(0.0, Math.Min(1.0,
                1 / (Math.Pow((t + 1), 2) + Math.Pow(p, 1.75))
                * Math.Pow(p, 2)
            ));

            return new UpgradeResultDTO
            {
                ProbIntact = 0,
                ProbDegrade = 1 - probDestroy,
                PropDestroy = probDestroy,
                DegradeFunction = "Uniform"
            };
        }

        private UpgradeResultDTO ItemDowngrade(
            UpgradeResultDTO skinResult,
            InventoryItem item,
            PriceInfo invItemPrice,
            double monetaryValue)
        {
            double m = invItemPrice.AveragePrice;
            double t = invItemPrice.TotalPrice + monetaryValue;
            double z = monetaryValue;
            double? i = item.Wear.CurrentPrice;
            if (i == null) return new UpgradeResultDTO()
            {
                FloatStart = item.Float,
                FloatEnd = item.Float,
                ProbIntact = 1,
                ProbDegrade = 0,
                PropDestroy = 0,
                DegradeFunction = "None"
            };
            // \left(1-\frac{m}{t}\right)\left(1-\frac{i}{t}\right)\left(1-\frac{m}{i+z}\right)
            double keepProb = Math.Max(0.0, Math.Min(1.0,
                (1 - (m / t)) *
                (1 - (i.Value / t)) *
                (1 - (m / (i.Value + z)))
            ));
            return new UpgradeResultDTO
            {
                FloatStart = item.Float,
                ProbIntact = keepProb * skinResult.PropDestroy,
                ProbDegrade = keepProb * skinResult.ProbDegrade,
                PropDestroy = 1 - keepProb,
                DegradeFunction = "Uniform"
            };
        }


        public async Task<UpgradeOutputDTO> UpgradeAsync(UpgradeInputDTO dto, int userId)
        {
            QueryOptions<User> options1 = new QueryOptions<User>()
                .Before(u => u.FairRandom);
            User? user = await _userRepository.GetByIdAsyncNew(userId, options1);
            if (user == null) throw new Exception("User not found.");

            QueryOptions<Skin> option2 = new QueryOptions<Skin>()
                .Before(s => s.Rarity, s => s.Item, s => s.Wears);
            Skin? skin = await _skinRepository.GetByIdAsyncNew(dto.TargetSkinId, option2);
            if (skin == null) throw new Exception("Skin not found.");
            IEnumerable<PriceHistory> priceHistories = _context.PriceHistories
                .Where(ph => ph.SkinId == skin.SkinId).OrderByDescending(ph => ph.PriceDate).Take(skin.Wears.Count);
            PriceInfo skinPrice = new PriceInfo(priceHistories.Select(ph => ph.PriceValue));
            if (skinPrice.PriceCount == 0) throw new Exception("Target skin has no price history.");

            QueryOptions<InventoryItem> options2 = new QueryOptions<InventoryItem>()
                .Before(i => i.Wear.Skin.Rarity)
                .Before(i => dto.InventoryItemIds.Contains(i.InventoryItemId))
                .Before(i => i.UserId == user.UserId)
                .After(i => i.Wear.WearClass.PriceHistories);
            IEnumerable<InventoryItem> invItems = await _inventoryItemRepository.GetAllAsyncNew(options2);
            if (invItems.Count() != dto.InventoryItemIds.Count)
                throw new Exception("One or more inventory items not found.");
            PriceInfo invItemPrice = new PriceInfo(invItems.Select(i => i.Wear.CurrentPrice));

            UpgradeOutputDTO output = new() {
                UpgradeResult = SkinUpgrade(
                    invItemPrice,
                    skinPrice,
                    dto.MonetaryValue
                )
            };

            foreach (InventoryItem inventoryItem in invItems)
            {
                UpgradeResultDTO itemUpgradeResult = ItemDowngrade(
                    output.UpgradeResult,
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
                output = await ExecuteUpgradeAsync(output, user, skin, invItems);
            }
            return output;
        }

        private async Task<UpgradeOutputDTO> ExecuteUpgradeAsync(UpgradeOutputDTO dto,
            User? user, Skin? skin, IEnumerable<InventoryItem>? invItems)
        {
            throw new NotImplementedException();
        }
    }
}
