namespace Shared.DTO.Helpers
{
    public class UpgradeInputDTO
    {
        public bool Preview { get; set; } = true;

        public List<int> InventoryItemIds { get; set; } = [];

        public double MonetaryValue { get; set; } = 0;

        public int TargetSkinId { get; set; }
    }
}
