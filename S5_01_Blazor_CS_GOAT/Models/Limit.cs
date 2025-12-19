namespace S5_01_Blazor_CS_GOAT.Models
{
    public class Limit
    {
        public int UserId { get; set; }
        public int LimitTypeId { get; set; }
        public double? LimitAmount { get; set; }
        public LimitType? LimitType { get; set; }
    }
}
