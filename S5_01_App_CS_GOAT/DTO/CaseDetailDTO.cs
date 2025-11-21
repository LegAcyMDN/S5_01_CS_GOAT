namespace S5_01_App_CS_GOAT.DTO
{
    public class CaseDetailDTO
    {
        // Element of Class Case
        public string CaseName { get; set; } = null!;
        public string? CaseImage { get; set; }
        public double CasePrice { get; set; }

        // Element of Class CaseContent
        public int Weight { get; set; }

        // Class Favorite
        public bool IsFavorite { get; set; } = false;
    }
}
