namespace Shared.DTO.Helpers
{
    public class MultipleCaseResultDTO
    {
        public Dictionary<int, SkinDTO> Skins { get; set; } = [];

        public int CaseId { get; set; }

        public double WalletBefore { get; set; }

        public double WalletAfter { get; set; }

        public CasePromoCodeDTO? PromoCode { get; set; }

        public LimitDTO? Limit { get; set; }

        public BanDTO? Ban { get; set; }

        public int ContentLength { get; set; } = 1;

        public List<IndividualCaseResultDTO> Results { get; set; } = [];
    }
}
