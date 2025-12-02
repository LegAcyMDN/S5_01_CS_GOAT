using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class TestFixture
    {
        // Ban test data
        public static List<BanDTO> GetBanDTOs()
        {
            return new List<BanDTO>
            {
                new BanDTO
                {
                    BanReason = "Violation des conditions d'utilisation",
                    BanDate = new DateTime(2025, 11, 1),
                    BanDuration = 7,
                    BanTypeName = "Temporary"
                },
                new BanDTO
                {
                    BanReason = "Comportement abusif envers d'autres utilisateurs",
                    BanDate = new DateTime(2025, 11, 15),
                    BanDuration = 30,
                    BanTypeName = "Standard"
                },
                new BanDTO
                {
                    BanReason = "Tentative de fraude",
                    BanDate = new DateTime(2025, 10, 20),
                    BanDuration = 14,
                    BanTypeName = "Temporary"
                },
                new BanDTO
                {
                    BanReason = "Activité suspecte détectée",
                    BanDate = new DateTime(2025, 12, 1),
                    BanDuration = 3,
                    BanTypeName = "Warning"
                },
                new BanDTO
                {
                    BanReason = "Spam et publicité non autorisée",
                    BanDate = new DateTime(2025, 11, 25),
                    BanDuration = 60,
                    BanTypeName = "Standard"
                }
            };
        }

        public static BanDTO GetSingleBanDTO()
        {
            return new BanDTO
            {
                BanReason = "Test ban reason",
                BanDate = new DateTime(2025, 12, 1),
                BanDuration = 7,
                BanTypeName = "Temporary"
            };
        }

        public static User GetAdminUser()
        {
            return new User
            {
                UserId = 1,
                Login = "admin",
                IsAdmin = true
            };
        }

        public static User GetNormalUser()
        {
            return new User
            {
                UserId = 2,
                Login = "user",
                IsAdmin = false
            };
        }

        public static BanType GetBanType()
        {
            return new BanType
            {
                BanTypeId = 1,
                BanTypeName = "Temporary"
            };
        }

        public static Ban GetBan()
        {
            BanDTO banDTO = GetSingleBanDTO();
            return new Ban
            {
                UserId = 2,
                BanTypeId = 1,
                BanReason = banDTO.BanReason,
                BanDate = banDTO.BanDate,
                BanDuration = banDTO.BanDuration,
                BanType = GetBanType()
            };
        }

        public static List<Ban> GetBans()
        {
            return new List<Ban> { GetBan() };
        }

        public static Ban GetUpdatedBan()
        {
            return new Ban
            {
                UserId = 2,
                BanTypeId = 1,
                BanReason = "Updated reason",
                BanDate = DateTime.Now,
                BanDuration = 14
            };
        }

        public static BanDTO GetEmptyBanDTO()
        {
            return new BanDTO();
        }

        // Case test data
        public static List<CaseDTO> GetCaseDTOs()
        {
            return new List<CaseDTO>
            {
                new CaseDTO
                {
                    CaseId = 1,
                    CaseName = "Assault Collection",
                    CaseImage = "assault_case.png",
                    CasePrice = 2.50,
                    Weight = 1000,
                    IsFavorite = false
                },
                new CaseDTO
                {
                    CaseId = 2,
                    CaseName = "Desert Eagle Collection",
                    CaseImage = "deagle_case.png",
                    CasePrice = 5.00,
                    Weight = 800,
                    IsFavorite = false
                },
                new CaseDTO
                {
                    CaseId = 3,
                    CaseName = "AWP Collection",
                    CaseImage = "awp_case.png",
                    CasePrice = 10.00,
                    Weight = 600,
                    IsFavorite = true
                }
            };
        }

        public static CaseDTO GetSingleCaseDTO()
        {
            return new CaseDTO
            {
                CaseId = 1,
                CaseName = "Test Case",
                CaseImage = "test_case.png",
                CasePrice = 3.99,
                Weight = 750,
                IsFavorite = false
            };
        }

        public static Case GetCase()
        {
            return new Case
            {
                CaseId = 1,
                CaseName = "Test Case",
                CaseImage = "test_case.png",
                CasePrice = 3.99
            };
        }

        public static List<Case> GetCases()
        {
            return new List<Case>
            {
                new Case
                {
                    CaseId = 1,
                    CaseName = "Assault Collection",
                    CaseImage = "assault_case.png",
                    CasePrice = 2.50
                },
                new Case
                {
                    CaseId = 2,
                    CaseName = "Desert Eagle Collection",
                    CaseImage = "deagle_case.png",
                    CasePrice = 5.00
                },
                new Case
                {
                    CaseId = 3,
                    CaseName = "AWP Collection",
                    CaseImage = "awp_case.png",
                    CasePrice = 10.00
                }
            };
        }

        public static Favorite GetFavorite()
        {
            return new Favorite
            {
                UserId = 2,
                CaseId = 1
            };
        }

        public static Favorite GetOtherUserFavorite()
        {
            return new Favorite
            {
                UserId = 1,
                CaseId = 1
            };
        }

        public static (int, int) GetFavoriteKey(int userId, int caseId)
        {
            return (userId, caseId);
        }

        public static (int, int) GetFavoriteKey1ForNormalUser()
        {
            return (2, 1);
        }

        public static (int, int) GetFavoriteKey2ForNormalUser()
        {
            return (2, 2);
        }

        public static (int, int) GetFavoriteKey3ForNormalUser()
        {
            return (2, 3);
        }

        // FairRandom test data
        public static List<FairRandomDTO> GetFairRandomDTOs()
        {
            return new List<FairRandomDTO>
            {
                new FairRandomDTO
                {
                    ServerSeed = "abc123seed456",
                    ServerHash = "hash123abc456def",
                    UserNonce = 1,
                    CombinedHash = "combined123hash456",
                    Fraction = 0.12345,
                    TransactionId = 1,
                    ItemWearId = null,
                    ItemUserId = null
                },
                new FairRandomDTO
                {
                    ServerSeed = "xyz789seed012",
                    ServerHash = "hash789xyz012ghi",
                    UserNonce = 2,
                    CombinedHash = "combined789hash012",
                    Fraction = 0.67890,
                    TransactionId = 2,
                    ItemWearId = null,
                    ItemUserId = null
                },
                new FairRandomDTO
                {
                    ServerSeed = "def456seed789",
                    ServerHash = "hash456def789jkl",
                    UserNonce = 3,
                    CombinedHash = "combined456hash789",
                    Fraction = 0.98765,
                    TransactionId = null,
                    ItemWearId = 1,
                    ItemUserId = 2
                }
            };
        }

        public static FairRandomDTO GetSingleFairRandomDTO()
        {
            return new FairRandomDTO
            {
                ServerSeed = "test123seed",
                ServerHash = "testhash123",
                UserNonce = 1,
                CombinedHash = "testcombinedhash",
                Fraction = 0.5,
                TransactionId = 1,
                ItemWearId = null,
                ItemUserId = null
            };
        }

        public static FairRandom GetFairRandom()
        {
            return new FairRandom
            {
                FairRandomId = 1,
                ServerSeed = "test123seed",
                ServerHash = "testhash123",
                UserNonce = 1,
                CombinedHash = "testcombinedhash",
                Fraction = 0.5
            };
        }

        public static List<FairRandom> GetFairRandoms()
        {
            return new List<FairRandom>
            {
                new FairRandom
                {
                    FairRandomId = 1,
                    ServerSeed = "abc123seed456",
                    ServerHash = "hash123abc456def",
                    UserNonce = 1,
                    CombinedHash = "combined123hash456",
                    Fraction = 0.12345
                },
                new FairRandom
                {
                    FairRandomId = 2,
                    ServerSeed = "xyz789seed012",
                    ServerHash = "hash789xyz012ghi",
                    UserNonce = 2,
                    CombinedHash = "combined789hash012",
                    Fraction = 0.67890
                },
                new FairRandom
                {
                    FairRandomId = 3,
                    ServerSeed = "def456seed789",
                    ServerHash = "hash456def789jkl",
                    UserNonce = 3,
                    CombinedHash = "combined456hash789",
                    Fraction = 0.98765
                }
            };
        }

        // GlobalNotification test data
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
