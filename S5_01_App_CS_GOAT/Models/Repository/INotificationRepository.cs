using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface INotificationRepository<TEntity> : IUserRelatedRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetRelevantAsync(
        int? userId,
        FilterOptions? filters = null,
        SortOptions? sorts = null);
    Task<TEntity?> GetDetailsAsync(int notificationId);
}
