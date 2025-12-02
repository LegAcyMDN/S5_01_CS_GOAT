using S5_01_App_CS_GOAT.Models.Partials;

namespace S5_01_App_CS_GOAT.DTO.Helpers;

public class CreateUserDTO
{
    public string Login { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Remember { get; set; }
}