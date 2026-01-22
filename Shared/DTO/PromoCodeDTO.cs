using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO;

/// <summary>
/// DTO pour les codes promotionnels avec support GetOptions
/// </summary>
public class PromoCodeDTO : IQueryableDTO
{
    // Implémentation de IQueryableDTO
    public static string? DefaultSortKey => "Code";
    public static SortingType? DefaultSortType => SortingType.Ascending;
    public static int DefaultPageSize => 25;
    public static bool CanSearch => true;
    public string? SearchTerm => Code;

    // Propriétés du PromoCode
    public int PromoCodeId { get; set; }
    public string Code { get; set; } = string.Empty;
    public int? RemainingUses { get; set; }
    public int? DiscountPercentage { get; set; }
    public double? DiscountAmount { get; set; }
    public DateTime ValidityStart { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public TimeSpan? RefreshDelay { get; set; }
    public int? CaseId { get; set; }
    public int? UserId { get; set; }

    // Navigation properties pour affichage
    public string? CaseName { get; set; }
    public string? UserLogin { get; set; }
}
