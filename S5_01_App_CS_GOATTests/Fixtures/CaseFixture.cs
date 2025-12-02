using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class CaseFixture
    {
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
    }
}
