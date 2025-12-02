using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class NotificationFixture
    {
        public static GlobalNotification GetGlobalNotification()
        {
            return new GlobalNotification
            {
                NotificationId = 1,
                NotificationSummary = "System Update",
                NotificationContent = "System will be updated tonight",
                NotificationDate = DateTime.Now,
                NotificationTypeId = 1,
                IncludeVisitors = true
            };
        }

        public static UserNotification GetUserNotification()
        {
            return new UserNotification
            {
                NotificationId = 2,
                NotificationSummary = "Welcome",
                NotificationContent = "Welcome to CS GOAT",
                NotificationDate = DateTime.Now,
                NotificationTypeId = 2,
                UserId = 2,
                IsRead = false
            };
        }

        public static UserNotification GetOtherUserNotification()
        {
            return new UserNotification
            {
                NotificationId = 3,
                NotificationSummary = "Admin Message",
                NotificationContent = "Message for admin",
                NotificationDate = DateTime.Now,
                NotificationTypeId = 2,
                UserId = 1,
                IsRead = false
            };
        }

        public static List<Notification> GetAllNotifications()
        {
            return new List<Notification>
            {
                new GlobalNotification
                {
                    NotificationId = 1,
                    NotificationSummary = "System Update",
                    NotificationContent = "System will be updated tonight",
                    NotificationDate = DateTime.Now,
                    NotificationTypeId = 1,
                    IncludeVisitors = true
                },
                new UserNotification
                {
                    NotificationId = 2,
                    NotificationSummary = "Welcome",
                    NotificationContent = "Welcome to CS GOAT",
                    NotificationDate = DateTime.Now,
                    NotificationTypeId = 2,
                    UserId = 2,
                    IsRead = false
                }
            };
        }

        public static List<GlobalNotification> GetGlobalNotifications()
        {
            return new List<GlobalNotification>
            {
                new GlobalNotification
                {
                    NotificationId = 1,
                    NotificationSummary = "System Update",
                    NotificationContent = "System will be updated tonight",
                    NotificationDate = DateTime.Now,
                    NotificationTypeId = 1,
                    IncludeVisitors = true
                }
            };
        }

        public static List<UserNotification> GetUserNotifications()
        {
            return new List<UserNotification>
            {
                new UserNotification
                {
                    NotificationId = 2,
                    NotificationSummary = "Welcome",
                    NotificationContent = "Welcome to CS GOAT",
                    NotificationDate = DateTime.Now,
                    NotificationTypeId = 2,
                    UserId = 2,
                    IsRead = false
                }
            };
        }

        public static NotificationDTO GetNotificationDTO()
        {
            return new NotificationDTO
            {
                NotificationId = 1,
                NotificationSummary = "System Update",
                NotificationContent = "System will be updated tonight",
                NotificationDate = DateTime.Now,
                NotificationTypeName = "System"
            };
        }

        public static List<NotificationDTO> GetNotificationDTOs()
        {
            return new List<NotificationDTO>
            {
                new NotificationDTO
                {
                    NotificationId = 1,
                    NotificationSummary = "System Update",
                    NotificationContent = "System will be updated tonight",
                    NotificationDate = DateTime.Now,
                    NotificationTypeName = "System"
                },
                new NotificationDTO
                {
                    NotificationId = 2,
                    NotificationSummary = "Welcome",
                    NotificationContent = "Welcome to CS GOAT",
                    NotificationDate = DateTime.Now,
                    NotificationTypeName = "User"
                }
            };
        }

        public static NotificationType GetNotificationType()
        {
            return new NotificationType
            {
                NotificationTypeId = 1,
                NotificationTypeName = "System"
            };
        }

        public static NotificationDTO GetSingleNotificationDTO()
        {
            return new NotificationDTO
            {
                NotificationSummary = "New Notification",
                NotificationContent = "This is a new notification",
                NotificationDate = DateTime.Now,
                NotificationTypeName = "System"
            };
        }

        public static NotificationDTO GetEmptyNotificationDTO()
        {
            return new NotificationDTO();
        }
    }
}
