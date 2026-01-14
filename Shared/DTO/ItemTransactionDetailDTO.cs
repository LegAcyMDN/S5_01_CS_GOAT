using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO
{
    public class ItemTransactionDetailDTO : IQueryableDTO
    {
        public int? InventoryItemId { get; set; }

        public DateTime TransactionDate { get; set; }

        public double WalletValue { get; set; }

        public DateTime? CancelledOn { get; set; } // day where the admin didn't approve the transaction

        public string? WearName { get; set; }

        public string? SkinName { get; set; }

        public string? ItemName { get; set; }

        public string? ItemTypeName { get; set; }

        public string? Uuid { get; set; }
        
        public string? RarityColor { get; set; }

        // IQueryableDTO implementation
        public static string? DefaultSortKey => "TransactionDate";
        public static SortingType? DefaultSortType => SortingType.Descending;
        public static int DefaultPageSize => 25;
        public static bool CanSearch => false;
        public string? SearchTerm { get; set; }
    }
}
