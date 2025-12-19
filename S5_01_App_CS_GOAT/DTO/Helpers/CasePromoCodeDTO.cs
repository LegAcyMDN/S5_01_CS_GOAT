namespace Shared.DTO.Helpers
{
    public class CasePromoCodeDTO
    {
        public string Code { get; set; } = null!;

        public int? CaseId { get; set; }

        public double? BasePrice { get; set; }

        public double? FinalPrice { get; set; }

        public int? RemainingUses { get; set; }

        public double DiscountAmount { get; set; }

        public int DiscountPercentage { get; set; }

        public DateTime ValidityStart { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? NextRefresh { get; set; }
    }
}
