namespace S5_01_Blazor_CS_GOAT.Models;

public class Case
{
    int CaseId { get; set; }
    string CaseName { get; set; }
    string CaseImage { get; set; }
    double CasePrice { get; set; }
    bool IsFavorite { get; set; }
}