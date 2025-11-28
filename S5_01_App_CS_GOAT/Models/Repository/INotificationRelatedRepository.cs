using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface INotificationRelatedRepository<TEntity> : IDataRepository<UserNotification, int>
    {
        Task<TEntity> GetNotificationTypeIdByNameAsync(string notificationTypeName);
    }
}
