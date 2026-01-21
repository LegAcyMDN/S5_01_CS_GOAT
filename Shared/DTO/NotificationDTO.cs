using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO
{
    public class NotificationDTO : IQueryableDTO
    {
        public static string? DefaultSortKey => "NotificationDate";

        public static SortingType? DefaultSortType => SortingType.Descending;

        public static int DefaultPageSize => 25;

        public static bool CanSearch => true;

        public string? SearchTerm => NotificationSummary;


        public int NotificationId { get; set; }

        public string NotificationSummary { get; set; } = null!;

        public string NotificationContent { get; set; } = null!;

        public DateTime NotificationDate { get; set; }

        public string NotificationTypeName { get; set; } = null!;
    }
}
