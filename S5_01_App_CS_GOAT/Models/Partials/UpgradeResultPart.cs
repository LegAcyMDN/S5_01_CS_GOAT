using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class UpgradeResult : IUserDependant
    {
        public int? DependantUserId { get => this.InventoryItem.UserId; }
    }
}