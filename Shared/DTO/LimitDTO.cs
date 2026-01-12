using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO
{
    public class LimitDTO : IQueryableDTO
    {
        public static string? DefaultSortKey => null;

        public static SortingType? DefaultSortType => null;

        public static int DefaultPageSize => 25;

        public static bool CanSearch => true;

        public string? SearchTerm => LimitTypeName;


        public double LimitAmount { get; set; } = 0; // amount of money the user don't want to spent

        public string LimitTypeName { get; set; } = null!; // name of why the user want to limit is spending
    }
}
