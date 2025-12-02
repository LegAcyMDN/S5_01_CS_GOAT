using System;
using System.Collections.Generic;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.DTO;

namespace S5_01_App_CS_GOATTests.Fixtures
{
    public static class InventoryItemFixture
    {
        public static InventoryItem GetInventoryItem()
        {
            return new InventoryItem
            {
                InventoryItemId = 1,
                UserId = 2,
                WearId = 1,
                Float = 0.15f,
                AcquiredOn = DateTime.Now.AddDays(-10),
                RemovedOn = null,
                IsFavorite = false
            };
        }

        public static InventoryItem GetOtherUserInventoryItem()
        {
            return new InventoryItem
            {
                InventoryItemId = 2,
                UserId = 1,
                WearId = 1,
                Float = 0.25f,
                AcquiredOn = DateTime.Now.AddDays(-5),
                RemovedOn = null,
                IsFavorite = false
            };
        }

        public static List<InventoryItem> GetInventoryItems()
        {
            return new List<InventoryItem>
            {
                new InventoryItem
                {
                    InventoryItemId = 1,
                    UserId = 2,
                    WearId = 1,
                    Float = 0.15f,
                    AcquiredOn = DateTime.Now.AddDays(-10),
                    RemovedOn = null,
                    IsFavorite = false
                },
                new InventoryItem
                {
                    InventoryItemId = 3,
                    UserId = 2,
                    WearId = 2,
                    Float = 0.08f,
                    AcquiredOn = DateTime.Now.AddDays(-5),
                    RemovedOn = null,
                    IsFavorite = true
                }
            };
        }

        public static InventoryItemDTO GetInventoryItemDTO()
        {
            return new InventoryItemDTO
            {
                InventoryItemId = 1,
                RarityColor = "#4B69FF",
                AcquiredOn = DateTime.Now.AddDays(-10),
                IsFavorite = false,
                Uuid = "test-uuid-1"
            };
        }

        public static InventoryItemDetailDTO GetInventoryItemDetailDTO()
        {
            return new InventoryItemDetailDTO
            {
                InventoryItemId = 1,
                Float = 0.15f,
                WearName = "Field-Tested",
                SkinName = "Redline",
                ItemName = "AK-47",
                ItemTypeName = "Rifle",
                RarityColor = "#4B69FF",
                RarityName = "Classified",
                AcquiredOn = DateTime.Now.AddDays(-10),
                IsFavorite = false,
                Uuid = "test-uuid-1"
            };
        }

        public static List<InventoryItemDTO> GetInventoryItemDTOs()
        {
            return new List<InventoryItemDTO>
            {
                new InventoryItemDTO
                {
                    InventoryItemId = 1,
                    RarityColor = "#4B69FF",
                    AcquiredOn = DateTime.Now.AddDays(-10),
                    IsFavorite = false,
                    Uuid = "test-uuid-1"
                },
                new InventoryItemDTO
                {
                    InventoryItemId = 3,
                    RarityColor = "#D2A679",
                    AcquiredOn = DateTime.Now.AddDays(-5),
                    IsFavorite = true,
                    Uuid = "test-uuid-3"
                }
            };
        }
    }
}
