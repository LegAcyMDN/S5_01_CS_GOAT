using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO;

public class BanDTO : IQueryableDTO
{
    public static string? DefaultSortKey => "BanDate";

    public static SortingType? DefaultSortType => SortingType.Descending;

    public static int DefaultPageSize => 25;

    public static bool CanSearch => true;

    public string? SearchTerm => BanReason;


    public int BanId { get; set; }

    public string BanReason { get; set; } = null!;

    public DateTime BanDate { get; set; } // start day of the ban

    public int BanDuration { get; set; } //  day duration of the ban

    public string BanTypeName { get; set; } = null!;

    public string BanTypeDescription { get; set; } = null!;
}
