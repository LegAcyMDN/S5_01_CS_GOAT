namespace Shared.DTO.Helpers
{
    public class IndividualCaseResultDTO
    {
        public InventoryItemDetailDTO Reward { get; set; } = null!;

        public int RandomTransactionId { get; set; }

        public FairRandomDTO Random { get; set; } = null!;

        public int[] Roller { get; set; } = Array.Empty<int>();
    }
}
