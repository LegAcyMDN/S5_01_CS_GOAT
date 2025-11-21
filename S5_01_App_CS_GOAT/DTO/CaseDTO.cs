namespace S5_01_App_CS_GOAT.DTO
{
    public class CaseDTO
    {
        public int CaseId { get; set; }
        public string CaseName { get; set; } = null!;
        public string? CaseImage { get; set; }
        public double CasePrice { get; set; }
        public bool IsFavorite { get; set; }
    }
}
