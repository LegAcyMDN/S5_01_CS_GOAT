using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class UpgradeResultManager(CSGOATDbContext context) : CrudRepository<UpgradeResult>(context)
    {
        public override async Task<UpgradeResult?> GetByKeyAsync(string str)
        {
            return null;
        }

        public async Task<IEnumerable<UpgradeResult>> GetByInventoryItemAsync(int inventoryItemId)
        {
            return await _context.UpgradeResults
                .Where(ur => ur.InventoryItemId == inventoryItemId)
                .ToListAsync();
        }

        public async Task<IEnumerable<UpgradeResult>> GetByRandomTransactionAsync(int transactionId)
        {
            return await _context.UpgradeResults
                .Where(ur => ur.TransactionId == transactionId)
                .ToListAsync();
        }
    }
}
