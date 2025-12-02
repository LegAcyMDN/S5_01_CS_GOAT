using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class PriceHistoryFixture
    {
        public static PriceHistory GetPriceHistory()
        {
            return new PriceHistory
            {
                PriceHistoryId = 1,
                PriceDate = DateTime.Now.AddDays(-30),
                PriceValue = 25.50,
                GuessDate = null,
                WearId = 1
            };
        }

        public static List<PriceHistory> GetPriceHistories()
        {
            return new List<PriceHistory>
            {
                new PriceHistory
                {
                    PriceHistoryId = 1,
                    PriceDate = DateTime.Now.AddDays(-30),
                    PriceValue = 25.50,
                    GuessDate = null,
                    WearId = 1
                },
                new PriceHistory
                {
                    PriceHistoryId = 2,
                    PriceDate = DateTime.Now.AddDays(-15),
                    PriceValue = 28.75,
                    GuessDate = null,
                    WearId = 1
                },
                new PriceHistory
                {
                    PriceHistoryId = 3,
                    PriceDate = DateTime.Now,
                    PriceValue = 30.00,
                    GuessDate = null,
                    WearId = 1
                }
            };
        }

        public static PriceHistoryDTO GetPriceHistoryDTO()
        {
            return new PriceHistoryDTO
            {
                PriceDate = DateTime.Now.AddDays(-30),
                PriceValue = 25.50,
                IsGuess = false
            };
        }

        public static List<PriceHistoryDTO> GetPriceHistoryDTOs()
        {
            return new List<PriceHistoryDTO>
            {
                new PriceHistoryDTO
                {
                    PriceDate = DateTime.Now.AddDays(-30),
                    PriceValue = 25.50,
                    IsGuess = false
                },
                new PriceHistoryDTO
                {
                    PriceDate = DateTime.Now.AddDays(-15),
                    PriceValue = 28.75,
                    IsGuess = false
                },
                new PriceHistoryDTO
                {
                    PriceDate = DateTime.Now,
                    PriceValue = 30.00,
                    IsGuess = false
                }
            };
        }
    }
}
