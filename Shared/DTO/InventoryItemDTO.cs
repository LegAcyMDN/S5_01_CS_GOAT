using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO;

public class InventoryItemDTO : IQueryableDTO
{
    public static string? DefaultSortKey => "AcquiredOn";

    public static SortingType? DefaultSortType => SortingType.Descending;

    public static int DefaultPageSize => 50;

    public static bool CanSearch => false;

    public string? SearchTerm => null;


    public int InventoryItemId { get; set; }

    public string RarityColor { get; set; } = null!;

    public bool IsFavorite { get; set; }

    public DateTime AcquiredOn { get; set; }

    public string? Uuid { get; set; }
}