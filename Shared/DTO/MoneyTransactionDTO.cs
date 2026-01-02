namespace Shared.DTO
{
    public class MoneyTransactionDTO
    {
        public DateTime TransactionDate { get; set; }

        public double WalletValue { get; set; }

        public DateTime? CancelledOn { get; set; } // day where the admin didn't approve the transaction

        public string PaymentMethod { get; set; } = null!;
    }
}
