namespace Shared.DTO.Helpers
{
    public class UpgradeOutputItemDTO
    {
        public InventoryItemDTO InventoryItem { get; set; } = new InventoryItemDTO();
        public UpgradeResultDTO UpgradeResult { get; set; } = new UpgradeResultDTO();
    }

    public class UpgradeOutputDTO
    {
        public bool Preview { get; set; } = true;

        public List<UpgradeOutputItemDTO> Items { get; set; } = [];

        public InventoryItemDetailDTO? ItemResult { get; set; }

        public FairRandomDTO? FairRandom { get; set; }

        public double FailProbability { get; set; }

        public double SuccessProbability => 1 - FailProbability;
    }
}
