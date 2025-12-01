namespace S5_01_App_CS_GOAT.DTO;

public class SkinDTO
{
    public string SkinName { get; set; } = null!;

    public string RarityName { get; set; } = null!;

    public string RarityColor { get; set; } = null!;

    public double BestPrice { get; set; }

    public double WorstPrice { get; set; }

    public string? AnyUuid { get; set; } // any wear of the skin

    public int? Weight { get; set; }
}