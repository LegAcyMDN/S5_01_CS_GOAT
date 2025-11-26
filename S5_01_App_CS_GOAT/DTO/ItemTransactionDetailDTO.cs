namespace S5_01_App_CS_GOAT.DTO
{
    public class ItemTransactionDetailDTO
    {
        public int InventoryItemId { get; set; }
        public DateTime TransactionDate { get; set; }
        public double WalletValue { get; set; }
        public DateTime? CancelledOn { get; set; }
        public string? WearName { get; set; } 
        public string? SkinName { get; set; }
        public string? ItemName { get; set; }
        public string? ItemTypeName { get; set; }
        public string? Uuid { get; set; }
        public string? RarityColor { get; set; }
    }
}
