namespace S5_01_App_CS_GOAT.DTO
{
    public class RandomTransactionDetailDTO
    {
        public int InventoryItemId { get; set; }

        public DateTime TransactionDate { get; set; }

        public double WalletValue { get; set; }

        public DateTime? CancelledOn { get; set; } // day where the admin didn't approve the transaction

        public string? WearName { get; set; }

        public string? SkinName { get; set; }

        public string? ItemName { get; set; }

        public string? ItemTypeName { get; set; }

        public string? Uuid { get; set; }
        
        public string? RarityColor { get; set; }

        public string UserSeed { get; set; } = null!;

        public CaseDTO? Case { get; set; }
    }
}
