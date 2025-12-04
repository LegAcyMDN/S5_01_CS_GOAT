using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class LimitTypeFixture
    {
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