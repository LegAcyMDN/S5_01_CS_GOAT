using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

public class UserNotificationManager(CSGOATDbContext context) : CrudRepository<UserNotification>(context), INotificationRelatedRepository<int?>
{
  

    public async Task<int?> GetNotificationTypeIdByNameAsync(string notificationTypeName)
    {
        NotificationType? notificationType = await _context.NotificationTypes
            .FirstOrDefaultAsync(nt => nt.NotificationTypeName == notificationTypeName);
        
        return notificationType?.NotificationTypeId;
    }
}