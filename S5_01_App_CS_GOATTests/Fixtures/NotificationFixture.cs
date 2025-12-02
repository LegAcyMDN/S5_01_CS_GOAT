using System;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class NotificationFixture
    {
        public static NotificationType GetNotificationType()
        {
            return new NotificationType
            {
                NotificationTypeId = 1,
                NotificationTypeName = "Info"
            };
        }

        public static NotificationDTO GetSingleNotificationDTO()
        {
            return new NotificationDTO
            {
                NotificationSummary = "Test Notification",
                NotificationContent = "This is a test notification content",
                NotificationDate = new DateTime(2025, 12, 1),
                NotificationTypeName = "Info"
            };
        }

        public static NotificationDTO GetEmptyNotificationDTO()
        {
            return new NotificationDTO();
        }

        public static GlobalNotification GetGlobalNotification()
        {
            return new GlobalNotification
            {
                NotificationId = 1,
                NotificationSummary = "Test Notification",
                NotificationContent = "This is a test notification content",
                NotificationDate = new DateTime(2025, 12, 1),
                NotificationTypeId = 1,
                IncludeVisitors = true,
                NotificationType = GetNotificationType()
            };
        }
    }
}
