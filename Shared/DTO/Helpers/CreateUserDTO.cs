namespace Shared.DTO.Helpers;

public class CreateUserDTO
{
    public string Login { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Remember { get; set; }
}