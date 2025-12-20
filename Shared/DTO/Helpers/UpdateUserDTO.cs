using Shared.Enum;

namespace Shared.DTO.Helpers
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
