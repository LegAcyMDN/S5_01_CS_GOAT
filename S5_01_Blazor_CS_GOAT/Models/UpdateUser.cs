namespace S5_01_Blazor_CS_GOAT.Models
{
    public class UpdateUser
    {
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? TwoFA { get; set; }
        public string? Seed { get; set; }
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
    }
}
