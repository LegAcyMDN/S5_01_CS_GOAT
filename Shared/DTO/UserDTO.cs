

using Shared.Enum;

namespace Shared.DTO;

public class UserDTO
{
    public int UserId { get; set; }

    public string Login { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool PhoneIsVerified { get; set; } // has user verified is phone number for his account

    public bool EmailIsVerified { get; set; } // has user verified is email for his account

    public TwoFAmethod TwoFA { get; set; } // method two factor authentication for the user account

    public DateTime CreationDate { get; set; }

    public string Seed { get; set; } = null!; // seed of the user for the fair random

    public double Wallet { get; set; }

    public bool IsSteamLogin { get; set; } // has the user link is account steam

    public bool IsAdmin { get; set; } = false;
}
