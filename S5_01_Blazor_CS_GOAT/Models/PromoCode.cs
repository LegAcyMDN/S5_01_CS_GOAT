namespace S5_01_Blazor_CS_GOAT.Models
{
    public class PromoCode
    {
        public int PromoCodeId { get; set; }
        public string Code { get; set; } = string.Empty;
        public int? RemainingUses { get; set; }
        public int? DiscountPercentage { get; set; }
        public double? DiscountAmount { get; set; }
        public DateTime ValidityStart { get; set; } = DateTime.Now;
        public DateTime? ExpiryDate { get; set; }
        public TimeSpan? RefreshDelay { get; set; }
        public int? CaseId { get; set; }
        public int? UserId { get; set; }
        
        // Navigation properties (optionnel pour affichage)
        public string? CaseName { get; set; }
        public string? UserLogin { get; set; }
    }
}
