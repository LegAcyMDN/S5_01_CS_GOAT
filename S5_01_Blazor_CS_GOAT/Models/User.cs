namespace S5_01_Blazor_CS_GOAT.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Login { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool PhoneIsVerified { get; set; }
        public bool EmailIsVerified { get; set; }
        public DateTime CreationDate { get; set; }
        public string Seed { get; set; } = string.Empty;
        public double Wallet { get; set; }
        public bool IsSteamLogin { get; set; }
    }
}
