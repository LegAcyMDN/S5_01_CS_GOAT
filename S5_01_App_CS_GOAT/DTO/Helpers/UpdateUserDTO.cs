using S5_01_App_CS_GOAT.Models.Partials;

namespace S5_01_App_CS_GOAT.DTO.Helpers
{
    public class UpdateUserDTO
    {
        public string? DisplayName { get; set; } = null!;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public TwoFAmethod? TwoFA { get; set; }

        public string? Seed { get; set; } = null!;

        public string? OldPassword { get; set; }

        public string? NewPassword { get; set; }
    }
}
