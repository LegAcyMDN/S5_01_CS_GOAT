namespace Shared.DTO
{
    public class RandomTransactionLiveFeedDTO
    {
        public DateTime TransactionDate { get; set; }

        public string ItemName { get; set; } = null!;

        public string SkinName { get; set; } = null!;

        public string RarityColor { get; set; } = null!;

        public string WearTypeAbbreviation { get; set; } = null!;

        public string Uuid { get; set; } = null!;
    }
}
