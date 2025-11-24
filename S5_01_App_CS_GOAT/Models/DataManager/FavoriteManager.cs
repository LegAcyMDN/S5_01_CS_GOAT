using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
 
        public class FavoriteManager(CSGOATDbContext context) : CrudRepository<Favorite>(context)
        {
            public override async Task<Favorite?> GetByKeyAsync(string name)
            {
                return await _context.Set<Favorite>().FirstOrDefaultAsync(c => c.User.DisplayName == name);
            }
        }
    }
