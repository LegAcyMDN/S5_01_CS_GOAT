using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

public class UserNotificationManager(CSGOATDbContext context) : CrudRepository<UserNotification>(context)
{
    public override async Task<UserNotification?> GetByKeyAsync(string name)
    {
        return await _context.Set<UserNotification>().FirstOrDefaultAsync(u => u.NotificationSummary == name);
    }
}