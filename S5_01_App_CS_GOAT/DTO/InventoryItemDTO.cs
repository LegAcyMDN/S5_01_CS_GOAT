namespace S5_01_App_CS_GOAT.DTO;

public class InventoryItemDTO
{
    public string? RarityColor{ get; set; }

    public bool IsFavorite { get; set; }

    public DateTime AcquiredOn { get; set; }

    public string? Uuid { get; set; }
}