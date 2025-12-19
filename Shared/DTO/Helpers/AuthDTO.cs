namespace Shared.DTO.Helpers
{
    public class AuthDTO
    {
        public int UserId { get; set; }

        public string? DisplayName { get; set; } = null!;

        public string JwtToken { get; set; } = null!;

        public TokenDTO? RememberToken { get; set; }
    }
}
