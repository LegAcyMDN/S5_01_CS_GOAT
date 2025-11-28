namespace S5_01_App_CS_GOAT.DTO;

public class InventoryItemDTO
{
    public int InventoryItemId { get; set; }

    public string RarityColor { get; set; } = null!;

    public bool IsFavorite { get; set; }

    public DateTime AcquiredOn { get; set; }

    public string? Uuid { get; set; }
}