using S5_01_App_CS_GOAT.Models.Partials;

namespace S5_01_App_CS_GOAT.DTO;

public class UserDetailDTO
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool PhoneIsVerified { get; set; }

    public bool EmailIsVerified { get; set; }

    public TwoFAmethod TwoFA { get; set; }

    public DateTime CreationDate { get; set; }

    public string Seed { get; set; } = null!;

    public double Wallet { get; set; }

    public bool IsSteamLogin { get; set; }
}
