using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.DTO.Helpers
{
    public class AuthDTO
    {
        public int UserId { get; set; }

        public string? DisplayName { get; set; } = null!;

        public string JwtToken { get; set; } = null!;

        public Token? RememberToken { get; set; }
    }
}
