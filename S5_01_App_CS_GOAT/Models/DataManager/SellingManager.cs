using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Manages the selling of inventory items and corresponding wallet transactions
    /// </summary>
    public class SellingManager : ISellingRepository
    {
        protected readonly CSGOATDbContext _context;
        protected readonly IMapper _mapper;
        protected readonly IPriceHistoryRepository _priceHistoryRepository;
        protected readonly IDataRepository<InventoryItem, int> _inventoryItemRepository;
        protected readonly IDataRepository<ItemTransaction, int> _itemTransactionRepository;

        public SellingManager(
            CSGOATDbContext context,
            IMapper mapper,
            IPriceHistoryRepository priceHistoryManager,
            IDataRepository<InventoryItem, int> inventoryItemRepository,
            IDataRepository<ItemTransaction, int> itemTransactionRepository
            )
        {
            _context = context;
            _mapper = mapper;
            _priceHistoryRepository = priceHistoryManager;
            _inventoryItemRepository = inventoryItemRepository;
            _itemTransactionRepository = itemTransactionRepository;
        }

        /// <summary>
        /// Sells an inventory item by ID, crediting the current price to the user's wallet
        /// </summary>
        /// <param name="invItemId">The inventory item ID to sell</param>
        /// <returns>HTTP status code indicating success (204), not found (404), or error</returns>
        public async Task<int> SellAsync(int invItemId)
        {
            QueryOptions<InventoryItem> options = new QueryOptions<InventoryItem>()
                .Before(i => i.User)
                .After(i => i.Wear.WearClass.PriceHistories);
            InventoryItem? invItem = await _inventoryItemRepository.GetByIdAsync(
                invItemId,
                options
            );
            return invItem == null ? StatusCodes.Status404NotFound : await SellAsync(invItem);
        }

        /// <summary>
        /// Sells an inventory item, marking it as removed and crediting the price to wallet
        /// </summary>
        /// <param name="invItem">The inventory item to sell</param>
        /// <returns>HTTP status code (204 success, 410 already removed, 503 price unavailable)</returns>
        /// <remarks>
        /// This method:
        /// 1. Checks if item is already removed (410 Gone)
        /// 2. Gets current price from price history (503 if unavailable)
        /// 3. Marks item as removed with current timestamp
        /// 4. Credits wallet with item price
        /// 5. Creates ItemTransaction record
        /// Uses database transaction for atomicity.
        /// </remarks>
        public async Task<int> SellAsync(InventoryItem invItem)
        {
            if (invItem.RemovedOn != null)
            {
                return StatusCodes.Status410Gone;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            double? currentPrice = invItem.Wear.CurrentPrice;
            if (currentPrice == null)
            {
                return StatusCodes.Status503ServiceUnavailable;
            }

            invItem.RemovedOn = DateTime.Now;
            invItem.User.Wallet += (double)currentPrice;
            var itemTransaction = new ItemTransaction()
            {
                WalletValue = (double)currentPrice,
                InventoryItemId = invItem.InventoryItemId,
                UserId = invItem.UserId,
            };
            _ = await _itemTransactionRepository.AddAsync(itemTransaction);
            await _inventoryItemRepository.UpdateAsync(invItem);
            _ = await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return StatusCodes.Status204NoContent;
        }
    }
}
