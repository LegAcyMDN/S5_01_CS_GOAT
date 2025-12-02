using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Services;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class User : IUserDependant
    {
        public int? DependantUserId { get => this.UserId; }
    }
}