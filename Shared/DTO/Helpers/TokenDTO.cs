namespace Shared.DTO.Helpers;

public class TokenDTO
{
    public int TokenId { get; set; }
    
    public required string TokenValue { get; set; }

    public DateTime TokenCreationDate { get; set; } = DateTime.Now;

    public DateTime TokenExpiry { get; set; }
    
    public int UserId { get; set; }
    
    public int TokenTypeId { get; set; }
}