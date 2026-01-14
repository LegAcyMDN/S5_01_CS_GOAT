using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO.Helpers
{
    public class UpgradeInputDTO
    {
        public bool Preview { get; set; } = true;

        public List<int> InventoryItemIds { get; set; } = new List<int>();

        public double MonetaryValue { get; set; } = 0;

        public int TargetSkinId { get; set; }
    }
}
