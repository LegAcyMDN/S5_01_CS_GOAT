using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class GlobalNotificationManager(CSGOATDbContext context) : CrudRepository<GlobalNotification>(context)
    {
        
    }
}
