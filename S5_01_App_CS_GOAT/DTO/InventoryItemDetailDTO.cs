namespace S5_01_App_CS_GOAT.DTO
{
    public class InventoryItemDetailDTO
    {
        public int InventoryItemId { get; set; }

        public float Float { get; set; }

        public bool IsFavorite { get; set; }

        public DateTime AcquiredOn { get; set; }

        public string? Uuid { get; set; }

        public string WearName { get; set; } = null!;

        public string SkinName { get; set; } = null!;

        public string ItemName { get; set; } = null!;

        public string ItemTypeName { get; set; } = null!;

        public string RarityColor { get; set; } = null!;

        public string RarityName { get; set;} = null!;

    }
}
