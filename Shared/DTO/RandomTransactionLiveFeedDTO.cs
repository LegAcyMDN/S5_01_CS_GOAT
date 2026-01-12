using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO
{
    public class RandomTransactionLiveFeedDTO : IQueryableDTO
    {
        public static string? DefaultSortKey => "TransactionDate";

        public static SortingType? DefaultSortType => SortingType.Descending;

        public static int DefaultPageSize => 10;

        public static bool CanSearch => false;

        public string? SearchTerm => null;


        public DateTime TransactionDate { get; set; }

        public string ItemName { get; set; } = null!;

        public string SkinName { get; set; } = null!;

        public string RarityColor { get; set; } = null!;

        public string WearTypeAbbreviation { get; set; } = null!;

        public string Uuid { get; set; } = null!;
    }
}
