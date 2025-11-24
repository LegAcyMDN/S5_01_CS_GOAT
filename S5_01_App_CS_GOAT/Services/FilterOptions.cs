namespace S5_01_App_CS_GOAT.Services;

public class FilterOptions // à mettre dans un dossier service
{
    public Dictionary<string, object?>? Filters { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
