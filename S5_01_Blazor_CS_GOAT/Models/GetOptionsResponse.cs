namespace S5_01_Blazor_CS_GOAT.Models;

/// <summary>
/// Modèle de réponse pour les endpoints qui supportent GetOptions (tri, filtrage, pagination)
/// </summary>
/// <typeparam name="T">Type du DTO</typeparam>
public class GetOptionsResponse<T>
{
    /// <summary>
    /// Noms des propriétés du DTO (+ searchTerm) (utilisables en paramètres de GET)
    /// </summary>
    public List<string> PropertyNames { get; set; } = new();

    /// <summary>
    /// Liste des objets retournés
    /// </summary>
    public List<T> Result { get; set; } = new();

    /// <summary>
    /// Filtres appliqués (clé = nom de propriété, valeur = liste des valeurs de filtre)
    /// </summary>
    public Dictionary<string, List<string>> Filters { get; set; } = new();

    /// <summary>
    /// Propriété sur laquelle le tri est effectué
    /// </summary>
    public string? SortKey { get; set; }

    /// <summary>
    /// Type de tri (asc, desc, search)
    /// </summary>
    public string? SortType { get; set; }

    /// <summary>
    /// Numéro de la page actuelle
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    /// Taille de la page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Nombre d'objets retournés dans cette page (entre 0 et pageSize)
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Nombre total de pages
    /// </summary>
    public int PageCount { get; set; }

    /// <summary>
    /// Nombre d'objets après filtres et avant pagination
    /// </summary>
    public int FilteredCount { get; set; }

    /// <summary>
    /// Nombre total d'objets avant filtres
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// Indique si la recherche est possible sur cet objet
    /// </summary>
    public bool CanSearch { get; set; }

    /// <summary>
    /// Type du DTO
    /// </summary>
    public string? DtoTypeName { get; set; }
}
