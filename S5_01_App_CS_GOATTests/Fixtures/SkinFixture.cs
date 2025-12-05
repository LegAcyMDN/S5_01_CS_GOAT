using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class SkinFixture
    {
        public static Skin GetSkin()
        {
            return new Skin
            {
                SkinId = 1,
                SkinName = "Redline",
                RarityId = 1,
                ItemId = 1,
                Rarity = new Rarity
                {
                    RarityId = 1,
                    RarityName = "Classified",
                    RarityColor = "#D32CE6"
                },
                Wears = new List<Wear>
                {
                    new Wear
                    {
                        WearId = 1,
                        SkinId = 1,
                        WearName = "Field-Tested",
                        Uuid = "test-uuid-1"
                    }
                }
            };
        }

        public static List<Skin> GetSkins()
        {
            return new List<Skin>
            {
                new Skin
                {
                    SkinId = 1,
                    SkinName = "Redline",
                    RarityId = 1,
                    ItemId = 1,
                    Rarity = new Rarity
                    {
                        RarityId = 1,
                        RarityName = "Classified",
                        RarityColor = "#D32CE6"
                    }
                },
                new Skin
                {
                    SkinId = 2,
                    SkinName = "Asiimov",
                    RarityId = 2,
                    ItemId = 2,
                    Rarity = new Rarity
                    {
                        RarityId = 2,
                        RarityName = "Covert",
                        RarityColor = "#EB4B4B"
                    }
                }
            };
        }

        public static SkinDTO GetSkinDTO()
        {
            return new SkinDTO
            {
                SkinName = "Redline",
                RarityName = "Classified",
                RarityColor = "#D32CE6",
                BestPrice = 10.50,
                WorstPrice = 25.00,
                AnyUuid = "test-uuid-1",
                Weight = 5
            };
        }

        public static List<SkinDTO> GetSkinDTOs()
        {
            return new List<SkinDTO>
            {
                new SkinDTO
                {
                    SkinName = "Redline",
                    RarityName = "Classified",
                    RarityColor = "#D32CE6",
                    BestPrice = 10.50,
                    WorstPrice = 25.00,
                    AnyUuid = "test-uuid-1",
                    Weight = 5
                },
                new SkinDTO
                {
                    SkinName = "Asiimov",
                    RarityName = "Covert",
                    RarityColor = "#EB4B4B",
                    BestPrice = 20.00,
                    WorstPrice = 50.00,
                    AnyUuid = "test-uuid-2",
                    Weight = 3
                }
            };
        }

        public static Case GetCaseWithSkins()
        {
            return new Case
            {
                CaseId = 1,
                CaseName = "Test Case",
                CasePrice = 2.50,
                CaseImage = "test-image.png",
                CaseContents = new List<CaseContent>
                {
                    new CaseContent
                    {
                        CaseId = 1,
                        SkinId = 1,
                        Weight = 5,
                        Skin = new Skin
                        {
                            SkinId = 1,
                            SkinName = "Redline",
                            RarityId = 1,
                            ItemId = 1,
                            Rarity = new Rarity
                            {
                                RarityId = 1,
                                RarityName = "Classified",
                                RarityColor = "#D32CE6"
                            },
                            Wears = new List<Wear>
                            {
                                new Wear
                                {
                                    WearId = 1,
                                    SkinId = 1,
                                    WearName = "Field-Tested",
                                    Uuid = "test-uuid-1",
                                    PriceHistories = new List<PriceHistory>()
                                }
                            }
                        }
                    },
                    new CaseContent
                    {
                        CaseId = 1,
                        SkinId = 2,
                        Weight = 3,
                        Skin = new Skin
                        {
                            SkinId = 2,
                            SkinName = "Asiimov",
                            RarityId = 2,
                            ItemId = 2,
                            Rarity = new Rarity
                            {
                                RarityId = 2,
                                RarityName = "Covert",
                                RarityColor = "#EB4B4B"
                            },
                            Wears = new List<Wear>
                            {
                                new Wear
                                {
                                    WearId = 2,
                                    SkinId = 2,
                                    WearName = "Factory New",
                                    Uuid = "test-uuid-2",
                                    PriceHistories = new List<PriceHistory>()
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
