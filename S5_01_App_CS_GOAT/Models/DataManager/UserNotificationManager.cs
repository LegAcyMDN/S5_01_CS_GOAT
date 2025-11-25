using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

public class UserNotificationManager(CSGOATDbContext context) : CrudRepository<UserNotification>(context), INotificationRelatedRepository<int?>
{
    public override async Task<UserNotification?> GetByKeyAsync(string name)
    {
        return await _context.Set<UserNotification>().FirstOrDefaultAsync(u => u.NotificationSummary == name);
    }

    public async Task<int?> GetNotificationTypeIdByNameAsync(string notificationTypeName)
    {
        NotificationType? notificationType = await _context.NotificationTypes
            .FirstOrDefaultAsync(nt => nt.NotificationTypeName == notificationTypeName);
        
        return notificationType?.NotificationTypeId;
    }
}