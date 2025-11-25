using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class NotificationSettingManager(CSGOATDbContext context) : CrudRepository<NotificationSetting>(context)
    {
        public override async Task<NotificationSetting?> GetByKeyAsync(string name)
        {
            return await _context.Set<NotificationSetting>().FirstOrDefaultAsync(ns => ns.NotificationType.NotificationTypeName == name);
        }

        public async Task<IEnumerable<NotificationSetting>> GetByUserIdAsync(int userId)
        {
            return await _context.Set<NotificationSetting>()
                .Where(ns => ns.UserId == userId)
                .Include(ns => ns.NotificationType)
                .ToListAsync();
        }
    }
}
