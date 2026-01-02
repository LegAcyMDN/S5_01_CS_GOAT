namespace Shared.DTO;

public class TokenDTO
{
    public int TokenId { get; set; }

    public required string TokenValue { get; set; }

    public DateTime? TokenExpiry { get; set; }

    public int UserId { get; set; }
}