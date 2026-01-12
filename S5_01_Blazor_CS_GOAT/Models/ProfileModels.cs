using Shared.Enum;

namespace S5_01_Blazor_CS_GOAT.Models
{
    /// <summary>
    /// Modèle pour la mise à jour du profil utilisateur
    /// </summary>
    public class UpdateUserModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public TwoFAmethod TwoFA { get; set; }
        public string? OldPassword { get; set; }
        public string Seed { get; set; } = string.Empty;
    }

    /// <summary>
    /// Modèle pour le changement de mot de passe
    /// </summary>
    public class ChangePasswordModel
    {
        public string OldPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
