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

        public static List<NotificationType> GetNotificationTypes()
        {
            return new List<NotificationType>
            {
                new NotificationType
                {
                    NotificationTypeId = 1,
                    NotificationTypeName = "System"
                },
                new NotificationType
                {
                    NotificationTypeId = 2,
                    NotificationTypeName = "User"
                }
            };
        }

        public static NotificationTypeDTO GetNotificationTypeDTO()
        {
            return new NotificationTypeDTO
            {
                NotificationTypeName = "System"
            };
        }

        public static List<NotificationTypeDTO> GetNotificationTypeDTOs()
        {
            return new List<NotificationTypeDTO>
            {
                new NotificationTypeDTO
                {
                    NotificationTypeName = "System"
                },
                new NotificationTypeDTO
                {
                    NotificationTypeName = "User"
                }
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

        // NotificationSetting fixtures
        public static NotificationSetting GetNotificationSetting()
        {
            return new NotificationSetting
            {
                UserId = 2,
                NotificationTypeId = 1,
                OnSite = true,
                ByEmail = true,
                ByPhone = false
            };
        }

        public static NotificationSetting GetOtherUserNotificationSetting()
        {
            return new NotificationSetting
            {
                UserId = 1,
                NotificationTypeId = 1,
                OnSite = true,
                ByEmail = false,
                ByPhone = false
            };
        }

        public static List<NotificationSetting> GetNotificationSettings()
        {
            return new List<NotificationSetting>
            {
                new NotificationSetting
                {
                    UserId = 2,
                    NotificationTypeId = 1,
                    OnSite = true,
                    ByEmail = true,
                    ByPhone = false
                },
                new NotificationSetting
                {
                    UserId = 2,
                    NotificationTypeId = 2,
                    OnSite = true,
                    ByEmail = false,
                    ByPhone = true
                }
            };
        }

        public static Dictionary<string, object> GetNotificationSettingPatchData()
        {
            return new Dictionary<string, object>
            {
                { "OnSite", false },
                { "ByEmail", true },
                { "ByPhone", true }
            };
        }

        public static (int, int) GetNotificationSettingKey(int userId, int notificationTypeId)
        {
            return (userId, notificationTypeId);
        }

        public static (int, int) GetNotificationSettingKeyForNormalUser()
        {
            return (2, 1);
        }
    }
}
