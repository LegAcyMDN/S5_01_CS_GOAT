using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class WearManager(CSGOATDbContext context) : CrudRepository<Wear>(context), IWearRelatedRepository<Wear>
    {
        public async Task<IEnumerable<Wear>> GetBy3dModelAsync(int modelID, FilterOptions? filters = null, SortOptions? sorts = null)
        {
            return await _context.Set<Wear>().Include(w => w.Skin).Where(w => w.Skin.ItemId == modelID).ToListAsync();
        }

       
        public override async Task<Wear?> GetByKeyAsync(string name)
        {
            return await _context.Set<Wear>().FirstOrDefaultAsync(w => w.WearName == name);
        }
    }
}
