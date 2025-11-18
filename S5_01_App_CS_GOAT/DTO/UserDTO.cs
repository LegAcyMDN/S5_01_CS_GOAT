namespace S5_01_App_CS_GOAT.DTO;

public class UserDTO
{
    public string Login { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? PhoneVerifiedOn { get; set; }

    public DateTime? EmailVerifiedOn { get; set; }

    public bool TwoFaIsPhone { get; set; }

    public bool TwoFaIsEmail { get; set; }

    public DateTime CreationDate { get; set; }

    public string Seed { get; set; } = null!;

    public double Wallet { get; set; }
}
