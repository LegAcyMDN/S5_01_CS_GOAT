using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class InventoryItemManager : CrudRepository<InventoryItem>, IToggleRepository<InventoryItem>
    {
        public InventoryItemManager(CSGOATDbContext context) : base(context)
        {
        }

        public override async Task<InventoryItem?> GetByKeyAsync(string name)
        {
            return await _context.InventoryItems.FirstOrDefaultAsync(item => item.Wear.WearName == name);
        }

        public async Task ToggleByIdsAsync(int userId, int wearId)
        {
            InventoryItem? item = await _context.InventoryItems.FindAsync(userId, wearId);
            if (item != null)
            {
                item.IsFavorite = !item.IsFavorite;
                await _context.SaveChangesAsync();
            }
        }
    }
}
