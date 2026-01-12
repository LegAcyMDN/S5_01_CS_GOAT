using Shared.Enum;
using Shared.Interfaces;

namespace Shared.DTO
{
    public class MoneyTransactionDTO : IQueryableDTO
    {
        public static string? DefaultSortKey => "TransactionDate";

        public static SortingType? DefaultSortType => SortingType.Descending;

        public static int DefaultPageSize => 25;

        public static bool CanSearch => false;

        public string? SearchTerm => null;


        public DateTime TransactionDate { get; set; }

        public double WalletValue { get; set; }

        public DateTime? CancelledOn { get; set; } // day where the admin didn't approve the transaction

        public string PaymentMethod { get; set; } = null!;
    }
}
