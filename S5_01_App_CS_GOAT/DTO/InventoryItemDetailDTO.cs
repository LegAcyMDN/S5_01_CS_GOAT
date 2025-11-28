namespace S5_01_App_CS_GOAT.DTO;

public class InventoryItemDetailDTO
{
    public int InventoryItemId { get; set; }

    public float Float { get; set; } // indication for the the quality of the wearname

    public bool IsFavorite { get; set; }

    public DateTime AcquiredOn { get; set; }

    public string? Uuid { get; set; }

    public string WearName { get; set; } = null!; // name of the wear for the item

    public string SkinName { get; set; } = null!; // name of the skin for the item

    public string ItemName { get; set; } = null!; // name of the item

    public string ItemTypeName { get; set; } = null!; // name of the type item

    public string RarityColor { get; set; } = null!; // return has an hexadecimal color

    public string RarityName { get; set;} = null!; // name of the rarity return by the rarity color
}
