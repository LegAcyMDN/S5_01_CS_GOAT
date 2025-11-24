using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class WearManager(CSGOATDbContext context) : CrudRepository<Wear>(context)
    {
        public override async Task<Wear?> GetByKeyAsync(string name)
        {
            return await _context.Set<Wear>().FirstOrDefaultAsync(w => w.WearName == name);
        }
    }
}
