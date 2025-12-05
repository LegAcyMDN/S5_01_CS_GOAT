using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class LimitFixture
    {
        // Limit fixtures
        public static Limit GetLimit()
        {
            return new Limit
            {
                UserId = 2,
                LimitTypeId = 1,
                LimitAmount = 100.00
            };
        }

        public static Limit GetOtherUserLimit()
        {
            return new Limit
            {
                UserId = 1,
                LimitTypeId = 1,
                LimitAmount = 200.00
            };
        }

        public static List<Limit> GetLimits()
        {
            return new List<Limit>
            {
                new Limit
                {
                    UserId = 2,
                    LimitTypeId = 1,
                    LimitAmount = 100.00
                },
                new Limit
                {
                    UserId = 2,
                    LimitTypeId = 2,
                    LimitAmount = 50.00
                }
            };
        }

        public static LimitDTO GetLimitDTO()
        {
            return new LimitDTO
            {
                LimitAmount = 150.00,
                LimitTypeName = "Daily",
                DurationName = "1 day"
            };
        }

        public static List<LimitDTO> GetLimitDTOs()
        {
            return new List<LimitDTO>
            {
                new LimitDTO
                {
                    LimitAmount = 100.00,
                    LimitTypeName = "Daily",
                    DurationName = "1 day"
                },
                new LimitDTO
                {
                    LimitAmount = 50.00,
                    LimitTypeName = "Weekly",
                    DurationName = "7 days"
                }
            };
        }

        public static LimitDTO GetUpdatedLimitDTO()
        {
            return new LimitDTO
            {
                LimitAmount = 250.00,
                LimitTypeName = "Daily",
                DurationName = "1 day"
            };
        }

        public static LimitDTO GetEmptyLimitDTO()
        {
            return new LimitDTO();
        }

        public static (int, int) GetLimitKey(int userId, int limitTypeId)
        {
            return (userId, limitTypeId);
        }

        public static (int, int) GetLimitKeyForNormalUser()
        {
            return (2, 1);
        }

        // LimitType fixtures
        public static LimitType GetLimitType()
        {
            return new LimitType
            {
                LimitTypeId = 1,
                LimitTypeName = "Daily",
                Duration = 1,
                DurationName = "Day"
            };
        }

        public static List<LimitType> GetLimitTypes()
        {
            return new List<LimitType>
            {
                new LimitType
                {
                    LimitTypeId = 1,
                    LimitTypeName = "Daily",
                    Duration = 1,
                    DurationName = "Day"
                },
                new LimitType
                {
                    LimitTypeId = 2,
                    LimitTypeName = "Weekly",
                    Duration = 7,
                    DurationName = "Week"
                },
                new LimitType
                {
                    LimitTypeId = 3,
                    LimitTypeName = "Monthly",
                    Duration = 30,
                    DurationName = "Month"
                }
            };
        }
    }
}
