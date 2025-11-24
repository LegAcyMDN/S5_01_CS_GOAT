using S5_01_App_CS_GOAT.Models.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class UpgradeResult : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }
    }
}