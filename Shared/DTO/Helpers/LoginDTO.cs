namespace Shared.DTO.Helpers
{
    public class LoginDTO
    {
        public string Identifier { get; set; } = null!;

        public string Password { get; set; } = null!;

        public int? Remember { get; set; }
    }
}
