using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class ItemManager(CSGOATDbContext context) : CrudRepository<Item>(context)
    {
        public override async Task<Item?> GetByKeyAsync(string name)
        {
            return await _context.Set<Item>().FirstOrDefaultAsync(i => i.ItemName == name );
        }
    }
}
