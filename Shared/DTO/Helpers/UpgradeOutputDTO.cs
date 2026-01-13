using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Helpers
{
    public class UpgradeOutputItemDTO
    {
        public InventoryItemDTO InventoryItem { get; set; } = new InventoryItemDTO();
        public UpgradeResultDTO UpgradeResult { get; set; } = new UpgradeResultDTO();
    }

    public class UpgradeOutputDTO
    {
        public List<UpgradeOutputItemDTO> Items { get; set; } = new List<UpgradeOutputItemDTO>();

        public InventoryItemDetailDTO? ItemResult { get; set; }

        public UpgradeResultDTO? UpgradeResult { get; set; }
    }
}
