using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
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
            InventoryItem? invItem = await _inventoryItemRepository.GetByIdAsync(
                invItemId, "User", "Wear.WearType.PriceHistories");
            if (invItem == null) return StatusCodes.Status404NotFound;
            return await SellAsync(invItem);
        }

        public async Task<int> SellAsync(InventoryItem invItem)
        {
            // TODO: Check if the user is not restricted by a ban
            // return StatusCodes.Status403Forbidden;

            if (invItem.RemovedOn != null) return StatusCodes.Status410Gone;
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();

            PriceHistory? lastPrice = invItem.LastPrice();
            double currentPrice = lastPrice != null ? lastPrice.PriceValue : 0;
            try
            {
                IEnumerable<PriceHistory>? newPrices = await _priceHistoryRepository.PredictWithAI(invItem, 7, true);
                if (newPrices == null) throw new Exception("AI prediction unsuccessful");
                currentPrice = newPrices.Min(p => p.PriceValue);
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }

            invItem.RemovedOn = DateTime.Now;
            invItem.User.Wallet += currentPrice;
            ItemTransaction itemTransaction = new ItemTransaction()
            {
                WalletValue = currentPrice,
                InventoryItemId = invItem.InventoryItemId,
                UserId = invItem.UserId,
            };
            await _itemTransactionRepository.AddAsync(itemTransaction);
            await transaction.CommitAsync();
            return StatusCodes.Status204NoContent;
        }
    }
}
