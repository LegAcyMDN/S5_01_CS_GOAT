using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class NotificationTypeManager(CSGOATDbContext context) : CrudRepository<NotificationType>(context), IDataRepository<NotificationType, int, string>
    {
       
    }
}
