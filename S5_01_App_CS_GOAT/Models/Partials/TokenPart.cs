using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class Token : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }
    }
}