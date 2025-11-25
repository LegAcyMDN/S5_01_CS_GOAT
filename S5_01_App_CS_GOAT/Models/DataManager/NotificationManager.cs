using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class NotificationManager(CSGOATDbContext context) 
        : CrudRepository<Notification>(context), INotificationRepository<Notification>
    {
        public override async Task<Notification?> GetByKeyAsync(string name)
        {
            return await _context.Set<Notification>()
                .FirstOrDefaultAsync(n => n.NotificationSummary == name);
        }

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
            if (filters?.Filters != null)
            {
                foreach (var filter in filters.Filters)
                {
                    if (filter.Key.Equals("notificationSummary", StringComparison.OrdinalIgnoreCase) && filter.Value is string summary)
                        query = query.Where(n => n.NotificationSummary.Contains(summary));
                    if (filter.Key.Equals("notificationTypeId", StringComparison.OrdinalIgnoreCase) && filter.Value is int typeId)
                        query = query.Where(n => n.NotificationTypeId == typeId);
                }

                if (filters.Page.HasValue && filters.PageSize.HasValue && filters.PageSize > 0)
                {
                    int skip = (filters.Page.Value - 1) * filters.PageSize.Value;
                    query = query.Skip(skip).Take(filters.PageSize.Value);
                }
            }

            if (sorts?.Sorts != null)
            {
                IOrderedQueryable<Notification>? ordered = null;
                foreach (var sort in sorts.Sorts)
                {
                    bool desc = sort.Value?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;
                    
                    if (sort.Key.Equals("notificationDate", StringComparison.OrdinalIgnoreCase))
                        ordered = ordered == null 
                            ? (desc ? query.OrderByDescending(n => n.NotificationDate) : query.OrderBy(n => n.NotificationDate))
                            : (desc ? ordered.ThenByDescending(n => n.NotificationDate) : ordered.ThenBy(n => n.NotificationDate));
                    
                    if (sort.Key.Equals("notificationSummary", StringComparison.OrdinalIgnoreCase))
                        ordered = ordered == null 
                            ? (desc ? query.OrderByDescending(n => n.NotificationSummary) : query.OrderBy(n => n.NotificationSummary))
                            : (desc ? ordered.ThenByDescending(n => n.NotificationSummary) : ordered.ThenBy(n => n.NotificationSummary));
                }

                if (ordered != null)
                    query = ordered;
            }

            return query;
        }
    }
}
