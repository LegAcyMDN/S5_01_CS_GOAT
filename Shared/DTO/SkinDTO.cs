using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO;

public class SkinDTO : IQueryableDTO
{
    public static string? DefaultSortKey => "Weight";

    public static SortingType? DefaultSortType => SortingType.Descending;

    public static int DefaultPageSize => 500;

    public static bool CanSearch => true;

    public string? SearchTerm => ItemName + " | " + SkinName + " | " + RarityName;


    public string SkinName { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public string RarityName { get; set; } = null!;

    public string RarityColor { get; set; } = null!;

    public string? AnyUuid { get; set; } // any wear of the skin

    public int? Weight { get; set; }
}