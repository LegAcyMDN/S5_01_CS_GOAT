using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class NotificationManager(CSGOATDbContext context) 
        : CrudRepository<Notification>(context), INotificationRepository<Notification>
    {
       

        public async Task<IEnumerable<Notification>> GetByUserAsync(
            int userId, 
            FilterOptions? filters = null, 
            SortOptions? sorts = null)
        {
            IQueryable<Notification> query = _context.Set<Notification>()
                .Where(n => n is UserNotification && ((UserNotification)n).UserId == userId)
                .Include(n => n.NotificationType);

            query = ApplyFiltersAndSorts(query, filters, sorts);
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetRelevantAsync(
            int? userId,
            FilterOptions? filters = null,
            SortOptions? sorts = null)
        {
            IQueryable<Notification> query = _context.Set<Notification>()
                .Include(n => n.NotificationType);

            if (userId.HasValue)
                query = query.Where(n => 
                    (n is GlobalNotification) || 
                    (n is UserNotification && ((UserNotification)n).UserId == userId.Value));
            else
                query = query.Where(n => n is GlobalNotification);

            query = ApplyFiltersAndSorts(query, filters, sorts);
            return await query.ToListAsync();
        }

        public async Task<Notification?> GetDetailsAsync(int notificationId)
        {
            return await _context.Set<Notification>()
                .Include(n => n.NotificationType)
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId);
        }

        private IQueryable<Notification> ApplyFiltersAndSorts(
            IQueryable<Notification> query,
            FilterOptions? filters = null,
            SortOptions? sorts = null)
        {
            // TODO: REMOVE
            throw new NotImplementedException("Filtering and sorting not implemented yet.");
            return query;
        }
    }
}
