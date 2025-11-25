namespace S5_01_App_CS_GOAT.Services;

public class FilterOptions
{
    public Dictionary<string, object?>? Filters { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
