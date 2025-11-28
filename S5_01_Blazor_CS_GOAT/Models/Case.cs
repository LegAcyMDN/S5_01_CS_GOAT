namespace S5_01_Blazor_CS_GOAT.Models;

public class Case
{
    public int CaseId { get; set; }
    public string CaseName { get; set; }
    public string CaseImage { get; set; }
    public double CasePrice { get; set; }
    public bool IsFavorite { get; set; }
}