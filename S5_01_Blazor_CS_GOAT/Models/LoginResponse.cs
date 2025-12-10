namespace S5_01_Blazor_CS_GOAT.Models;

public class LoginResponse
{
    public int UserId { get; set; }
    public string? DisplayName { get; set; }
    public string JwtToken { get; set; } = string.Empty;
    public RememberToken? RememberToken { get; set; }
}

public class RememberToken
{
    public string Value { get; set; } = string.Empty;
    public DateTime Expiry { get; set; }
}