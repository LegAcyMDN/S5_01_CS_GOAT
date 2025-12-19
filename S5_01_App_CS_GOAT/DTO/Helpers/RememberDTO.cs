namespace Shared.DTO.Helpers
{
    public class RememberDTO
    {
        public int UserId { get; set; }

        public int TokenId { get; set; }

        public string Token { get; set; } = null!;
    }
}
