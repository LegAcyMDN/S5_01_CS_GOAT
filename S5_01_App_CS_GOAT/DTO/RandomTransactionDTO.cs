namespace S5_01_App_CS_GOAT.DTO
{
    public class RandomTransactionDTO
    {
        public int InventoryItemId { get; set; }

        public DateTime TransactionDate { get; set; }

        public double WalletValue { get; set; }

        public DateTime? CancelledOn { get; set; } // day where the admin didn't approve the transaction
    }
}




