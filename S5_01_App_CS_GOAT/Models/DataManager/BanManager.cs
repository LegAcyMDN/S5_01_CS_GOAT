using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class BanManager(CSGOATDbContext context) : CrudRepository<Ban>(context)
    {
        public override async Task<Ban?> GetByKeyAsync(string str)
        {
            return await _context.Set<Ban>().FirstOrDefaultAsync(b => b.User.DisplayName == str);
        }
    }
}
