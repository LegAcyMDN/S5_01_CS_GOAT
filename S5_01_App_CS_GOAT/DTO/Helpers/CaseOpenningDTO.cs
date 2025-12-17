namespace S5_01_App_CS_GOAT.DTO.Helpers
{
    public class CaseOpenningDTO
    {
        public int CaseId { get; set; }

        public string? PromoCode { get; set; }

        public int Quantity { get; set; } = 1;

        public int RaffleRollerLength { get; set; } = 0;
    }
}
