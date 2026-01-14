using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<int> SellAsync(int invItemId)
        {
            QueryOptions<InventoryItem> options = new QueryOptions<InventoryItem>()
                .Before(i => i.User)
                .After(i => i.Wear.WearClass.PriceHistories);
            InventoryItem? invItem = await _inventoryItemRepository.GetByIdAsyncNew(
                invItemId,
                options
            );
            if (invItem == null) return StatusCodes.Status404NotFound;
            return await SellAsync(invItem);
        }

        public async Task<int> SellAsync(InventoryItem invItem)
        {
            // TODO: Check if the user is not restricted by a ban
            // return StatusCodes.Status403Forbidden;

            if (invItem.RemovedOn != null) return StatusCodes.Status410Gone;
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            double? currentPrice = invItem.Wear.CurrentPrice;
            if (currentPrice == null) return StatusCodes.Status503ServiceUnavailable;

            invItem.RemovedOn = DateTime.Now;
            invItem.User.Wallet += (double)currentPrice;
            ItemTransaction itemTransaction = new ItemTransaction()
            {
                WalletValue = (double)currentPrice,
                InventoryItemId = invItem.InventoryItemId,
                UserId = invItem.UserId,
            };
            await _itemTransactionRepository.AddAsync(itemTransaction);
            await _inventoryItemRepository.UpdateAsync(invItem);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return StatusCodes.Status204NoContent;
        }
    }
}
