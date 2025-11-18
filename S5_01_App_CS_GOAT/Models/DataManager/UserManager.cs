using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager;

public class UserManager(CSGOATDbContext context) : CrudRepository<User>(context)
{    public override async Task<User?> GetByKeyAsync(string name)
    {
        return await _context.Set<User>().FirstOrDefaultAsync(i => i.DisplayName == name);
    }
}
