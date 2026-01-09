namespace Shared.DTO;

public class SkinDTO
{
    public string SkinName { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public string RarityName { get; set; } = null!;

    public string RarityColor { get; set; } = null!;

    public string? AnyUuid { get; set; } // any wear of the skin

    public int? Weight { get; set; }
}