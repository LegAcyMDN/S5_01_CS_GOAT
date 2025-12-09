using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO.Helpers;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class WearFixture
    {
        public static Wear GetWear()
        {
            return new Wear
            {
                WearId = 1,
                SkinId = 1,
                WearTypeId = 1,
                WearFloat = 0.25f,
                Uuid = "test-uuid-1",
                Skin = new Skin
                {
                    SkinId = 1,
                    SkinName = "Redline",
                    ItemId = 1,
                    Item = new Item
                    {
                        ItemId = 1,
                        ItemName = "AK-47",
                        ItemModel = "ak47_model",
                        ItemTypeId = 1
                    }
                },
                WearType = new WearType
                {
                    WearTypeId = 1,
                    WearTypeName = "Field-Tested",
                    PriceHistories = new List<PriceHistory>
                    {
                        new PriceHistory
                        {
                            PriceHistoryId = 1,
                            SkinId = 1,
                            WearTypeId = 1,
                            PriceValue = 150.0
                        }
                    }
                }
            };
        }

        public static Wear GetWearWithFullData()
        {
            return new Wear
            {
                WearId = 1,
                SkinId = 1,
                WearTypeId = 1,
                WearFloat = 0.25f,
                Uuid = "test-uuid-1",
                Skin = new Skin
                {
                    SkinId = 1,
                    SkinName = "Redline",
                    ItemId = 1,
                    Item = new Item
                    {
                        ItemId = 1,
                        ItemName = "AK-47",
                        ItemModel = "ak47_model",
                        ItemTypeId = 1
                    }
                }
            };
        }

        public static List<Wear> GetWears()
        {
            return new List<Wear>
            {
                new Wear
                {
                    WearId = 1,
                    SkinId = 1,
                    WearTypeId = 1,
                    WearFloat = 0.25f,
                    Uuid = "test-uuid-1"
                },
                new Wear
                {
                    WearId = 2,
                    SkinId = 1,
                    WearTypeId = 2,
                    WearFloat = 0.75f,
                    Uuid = "test-uuid-2"
                }
            };
        }

        public static ModelDTO GetModelDTO()
        {
            return new ModelDTO
            {
                Uuid = "test-uuid-1",
                UvType = 1,
                ItemModel = "ak47_model",
                Texture = new byte[]?[]
                {
                    new byte[] { 1, 2, 3, 4 },
                    new byte[] { 5, 6, 7, 8 }
                }
            };
        }
    }
}
