using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class BanFixture
    {
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
    }
}
