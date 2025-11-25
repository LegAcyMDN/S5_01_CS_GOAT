using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class InventoryItemManager : CrudRepository<InventoryItem>, IUserRelatedRepository<InventoryItem>, IToggleRepository<InventoryItem>
    {
        public InventoryItemManager(CSGOATDbContext context) : base(context)
        {
        }

        public override async Task<InventoryItem?> GetByKeyAsync(string name)
        {
            return await _context.InventoryItems.FirstOrDefaultAsync(item => item.Wear.WearName == name);
        }

        public async Task<IEnumerable<InventoryItem>> GetByUserAsync(int userId, FilterOptions? filters = null, SortOptions? sorts = null)
        {
            IQueryable<InventoryItem> query = _context.InventoryItems.Where(item => item.UserId == userId);

            // Apply filters and sorts if provided
            if (filters?.Filters != null)
            {
                foreach (var filter in filters.Filters)
                {
                    query = query.Where(item => EF.Property<object>(item, filter.Key).Equals(filter.Value));
                }
            }

            if (sorts?.Sorts != null)
            {
                foreach (var sort in sorts.Sorts)
                {
                    query = query.OrderBy(item => EF.Property<object>(item, sort.Key));
                }
            }

            return await query.ToListAsync();
        }

        public async Task ToggleAsync(int id)
        {
            InventoryItem? item = await _context.InventoryItems.FindAsync(id);
            if (item != null)
            {
                item.IsFavorite = !item.IsFavorite;
                await _context.SaveChangesAsync();
            }
        }
    }
}
