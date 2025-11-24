using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;


namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class FairRandomManager(CSGOATDbContext context) : CrudRepository<FairRandom>(context)
    {
        public override async Task<FairRandom?> GetByKeyAsync(string name)
        {
            return await _context.Set<FairRandom>().FirstOrDefaultAsync(c => c.CombinedHash == name);
        }
    }
}
