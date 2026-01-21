using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Primitives;
using Shared.Enum;
using Shared.Interfaces;

namespace S5_01_App_CS_GOAT.Services;

/// <summary>
/// Provides query parsing, filtering, sorting, and pagination capabilities for DTOs
/// </summary>
/// <typeparam name="Tdto">The DTO type that implements IQueryableDTO</typeparam>
/// <remarks>
/// This class parses HTTP query strings and applies filtering, sorting (including search-based sorting using Levenshtein distance),
/// and pagination to collections of DTOs. It supports both ascending/descending sorts and fuzzy text search.
/// </remarks>
public class GetOptions<Tdto>
    where Tdto : class, IQueryableDTO
{
    /// <summary>
    /// Maps various string aliases to their corresponding SortingType enumeration values
    /// </summary>
    private static readonly Dictionary<List<string>, SortingType> SortingTypeMapping = new()
    {
        { new() { "a", "asc", "ascend", "ascending" }, SortingType.Ascending },
        { new() { "d", "des", "desc", "descend", "descending" }, SortingType.Descending },
        { new() { "s", "search", "levenshtein" }, SortingType.Search }
    };

    /// <summary>
    /// Reflection binding flags for case-insensitive public instance property access
    /// </summary>
    private static readonly BindingFlags BindingFlags =
        BindingFlags.IgnoreCase |
        BindingFlags.Public |
        BindingFlags.Instance;

    /// <summary>
    /// Reserved query parameter keywords that are not treated as filter properties
    /// </summary>
    private static readonly List<string> KeyWords =
    [
        "sorttype", "sortkey", "page", "pagenumber", "pagesize"
    ];


    private IQueryCollection? _query;
    private string? _sortKey;
    private SortingType? _sortType;
    private int _pageNumber = 1;
    private int? _pageSize;

    /// <summary>
    /// Gets the list of all public instance property names for the DTO type
    /// </summary>
    public List<string> PropertyNames =>
        typeof(Tdto).GetProperties(BindingFlags).Select(p => p.Name).ToList();

    /// <summary>
    /// Gets or sets the query collection and automatically parses it when set
    /// </summary>
    private IQueryCollection? Query
    {
        get => _query;
        set
        {
            _query = value;
            ParseQuery();
        }
    }

    /// <summary>
    /// Gets or sets the result collection of DTOs to be filtered, sorted, and paginated
    /// </summary>
    public IEnumerable<Tdto> Result { get; set; } = Enumerable.Empty<Tdto>();

    /// <summary>
    /// Gets the dictionary of filters where keys are property names and values are lists of allowed values
    /// </summary>
    public Dictionary<string, List<string?>> Filters
    {
        get;
        private set;
    } = [];

    /// <summary>
    /// Gets or sets the property name to sort by. Defaults to the DTO's default sort key.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the sort key doesn't match any DTO property (except for search sorting)</exception>
    public string? SortKey
    {
        get => _sortKey ?? Tdto.DefaultSortKey;
        set
        {
            if (SortType != SortingType.Search &&
                !PropertyNames.Any(p => p.Equals(value, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Invalid sort key: {value}");
            }

            _sortKey = value;
        }
    }

    /// <summary>
    /// Gets or sets the type of sorting to apply (Ascending, Descending, or Search). Defaults to the DTO's default sort type.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when attempting to use Search sorting on a DTO that doesn't support it</exception>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortingType? SortType
    {
        get => _sortType ?? Tdto.DefaultSortType;
        set
        {
            if (value == SortingType.Search && !CanSearch)
            {
                throw new ArgumentException("Search sorting type is not supported for this DTO.");
            }

            _sortType = value;
        }
    }

    /// <summary>
    /// Gets or sets the current page number (1-based). Must be greater than zero.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when value is less than or equal to zero</exception>
    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("PageNumber must be greater than zero.");
            }

            _pageNumber = value;
        }
    }

    /// <summary>
    /// Gets or sets the number of items per page. Defaults to the DTO's default page size. Must be greater than zero.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when value is less than or equal to zero</exception>
    public int PageSize
    {
        get => _pageSize ?? Tdto.DefaultPageSize;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("PageSize must be greater than zero.");
            }

            _pageSize = value;
        }
    }

    /// <summary>
    /// Gets the number of items in the current result set (after filtering, sorting, and pagination)
    /// </summary>
    public int Count => Result.Count();

    /// <summary>
    /// Gets the total number of pages based on TotalCount and PageSize
    /// </summary>
    public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Gets the number of items after filtering but before pagination
    /// </summary>
    public int FilteredCount { get; private set; } = 0;

    /// <summary>
    /// Gets the total number of items before any filtering or pagination
    /// </summary>
    public int TotalCount { get; private set; } = 0;

    /// <summary>
    /// Gets whether the DTO type supports search-based sorting
    /// </summary>
    public bool CanSearch => Tdto.CanSearch;

    /// <summary>
    /// Gets the name of the DTO type
    /// </summary>
    public string DtoTypeName => typeof(Tdto).Name;

    /// <summary>
    /// Applies all configured filters to the result collection
    /// </summary>
    /// <param name="result">The collection to filter</param>
    /// <returns>The filtered collection where each item's property values match one of the allowed filter values</returns>
    public IEnumerable<Tdto> ApplyFilters(IEnumerable<Tdto> result)
    {
        if (!result.Any())
        {
            return result;
        }

        // Apply each filter by checking if the property value is in the allowed list
        foreach (KeyValuePair<string, List<string?>> filter in Filters)
        {
            result = result.Where(item =>
            {
                string? propertyValue = item.GetType().GetProperty(filter.Key, BindingFlags)?.GetValue(item, null)?.ToString();
                return filter.Value.Contains(propertyValue);
            });
        }
        return result;
    }

    /// <summary>
    /// Applies sorting to the result collection based on SortType and SortKey
    /// </summary>
    /// <param name="result">The collection to sort</param>
    /// <returns>The sorted collection</returns>
    public IEnumerable<Tdto> ApplySorting(IEnumerable<Tdto> result)
    {
        if (!result.Any())
        {
            return result;
        }

        if (SortKey == null)
        {
            return result;
        }

        // Apply sorting based on the configured sort type
        switch (SortType ?? SortingType.Ascending)
        {
            case SortingType.Ascending:
                return result.OrderBy(item => item.GetType().GetProperty(SortKey, BindingFlags)?.GetValue(item, null));
            case SortingType.Descending:
                return result.OrderByDescending(item => item.GetType().GetProperty(SortKey, BindingFlags)?.GetValue(item, null));
            case SortingType.Search:
                return Search(result, SortKey);
            default:
                throw new Exception(); // Unreachable
        }
    }

    /// <summary>
    /// Applies pagination to the result collection
    /// </summary>
    /// <param name="result">The collection to paginate</param>
    /// <returns>The paginated subset of items for the current page</returns>
    public IEnumerable<Tdto> ApplyPaging(IEnumerable<Tdto> result)
    {
        return result.Skip((PageNumber - 1) * PageSize).Take(PageSize);
    }

    /// <summary>
    /// Applies all operations (filtering, sorting, pagination) to the current Result collection
    /// </summary>
    /// <returns>This GetOptions instance for method chaining</returns>
    private GetOptions<Tdto> Apply()
    {
        if (Result == null || Count == 0)
        {
            return this;
        }

        // Track counts and apply operations in sequence
        TotalCount = Result.Count();
        Result = ApplyFilters(Result);
        FilteredCount = Result.Count();
        Result = ApplySorting(Result);
        Result = ApplyPaging(Result);
        return this;
    }

    /// <summary>
    /// Sets the result collection and applies all operations to it
    /// </summary>
    /// <param name="result">The collection to process</param>
    /// <returns>This GetOptions instance for method chaining</returns>
    public GetOptions<Tdto> Apply(IEnumerable<Tdto>? result)
    {
        if (result == null)
        {
            return this;
        }

        Result = result;
        _ = Apply();
        return this;
    }

    /// <summary>
    /// Sorts the collection by similarity to the search term using Levenshtein distance
    /// </summary>
    /// <param name="result">The collection to search and sort</param>
    /// <param name="searchTerm">The term to search for</param>
    /// <returns>The collection ordered by relevance (closest match first)</returns>
    public static IEnumerable<Tdto> Search(IEnumerable<Tdto> result, string searchTerm)
    {
        if (!result.Any())
        {
            return result;
        }

        if (!Tdto.CanSearch)
        {
            return result;
        }

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return result;
        }

        // Order by Levenshtein distance (smaller distance = better match)
        result = result.OrderBy(item => ComputeLevenshteinDistance(
            item.SearchTerm?.ToLower() ?? "",
            searchTerm.ToLower()
        ));
        return result;
    }

    /// <summary>
    /// Computes the Levenshtein distance (edit distance) between two strings
    /// </summary>
    /// <param name="source">The first string</param>
    /// <param name="target">The second string</param>
    /// <returns>The minimum number of single-character edits (insertions, deletions, or substitutions) required to change source into target</returns>
    private static int ComputeLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
        {
            return string.IsNullOrEmpty(target) ? 0 : target.Length;
        }

        if (string.IsNullOrEmpty(target))
        {
            return source.Length;
        }

        int sourceLength = source.Length;
        int targetLength = target.Length;

        // Initialize the distance matrix
        int[,] distance = new int[sourceLength + 1, targetLength + 1];

        // Set up base cases (transforming empty string)
        for (int i = 0; i <= sourceLength; i++)
        {
            distance[i, 0] = i;
        }

        for (int j = 0; j <= targetLength; j++)
        {
            distance[0, j] = j;
        }

        // Fill in the distance matrix using dynamic programming
        for (int i = 1; i <= sourceLength; i++)
        {
            for (int j = 1; j <= targetLength; j++)
            {
                int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;

                distance[i, j] = Math.Min(
                    Math.Min(
                        distance[i - 1, j] + 1,      // deletion
                        distance[i, j - 1] + 1),     // insertion
                    distance[i - 1, j - 1] + cost    // substitution
                );
            }
        }

        return distance[sourceLength, targetLength];
    }

    /// <summary>
    /// Initializes a new instance of the GetOptions class
    /// </summary>
    public GetOptions() { }

    /// <summary>
    /// Initializes a new instance with a query collection and result set
    /// </summary>
    /// <param name="query">The query collection to parse</param>
    /// <param name="result">The initial result collection</param>
    public GetOptions(IQueryCollection? query, IEnumerable<Tdto>? result)
    { ParseQuery(query); _ = Apply(result); }

    /// <summary>
    /// Initializes a new instance with an HTTP request and result set
    /// </summary>
    /// <param name="request">The HTTP request containing the query</param>
    /// <param name="result">The initial result collection</param>
    public GetOptions(HttpRequest? request, IEnumerable<Tdto>? result)
        : this(request?.Query, result) { }

    /// <summary>
    /// Initializes a new instance with a query collection
    /// </summary>
    /// <param name="query">The query collection to parse</param>
    public GetOptions(IQueryCollection? query)
        : this(query, null) { }

    /// <summary>
    /// Initializes a new instance with an HTTP request
    /// </summary>
    /// <param name="request">The HTTP request containing the query</param>
    public GetOptions(HttpRequest? request)
        : this(request, null) { }

    /// <summary>
    /// Initializes a new instance with a result collection
    /// </summary>
    /// <param name="result">The initial result collection</param>
    public GetOptions(IEnumerable<Tdto>? result)
        : this(query: null, result) { }

    /// <summary>
    /// Parses the query collection from an HTTP request
    /// </summary>
    /// <param name="request">The HTTP request containing the query to parse</param>
    public void ParseQuery(HttpRequest request)
    { ParseQuery(request.Query); }

    /// <summary>
    /// Parses the query collection to extract filters, sorting, and pagination parameters
    /// </summary>
    /// <param name="query">The query collection to parse. If null, uses the existing Query property.</param>
    /// <exception cref="ArgumentException">Thrown when invalid filter keys or incomplete sorting parameters are provided</exception>
    public void ParseQuery(IQueryCollection? query = null)
    {
        if (query != null)
        {
            Query = query;
        }

        if (Query == null)
        {
            return;
        }

        // Parse filters from query parameters (excluding reserved keywords)
        foreach (KeyValuePair<string, StringValues> item in Query)
        {
            string key = item.Key.ToLower();
            if (KeyWords.Contains(key))
            {
                continue;
            }

            if (!PropertyNames.Any(p => p.Equals(key, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"Invalid filter key: {key}");
            }

            Filters[key] = item.Value.ToList();
        }

        // Parse sorting parameters (sorttype and sortkey must be provided together)
        if (Query.TryGetValue("sorttype", out StringValues sortType))
        {
            SortType = SortingTypeMapping.SelectMany(kv => kv.Key
                .Where(s => s.Equals(sortType.First(), StringComparison.OrdinalIgnoreCase))
                .Select(_ => kv.Value))
                .First();
        }

        if (Query.TryGetValue("sortkey", out StringValues sortKey))
        {
            SortKey = sortKey.First()!;
        }

        if ((SortType != null ? 1 : 0) + (SortKey != null ? 1 : 0) == 1)
        {
            throw new ArgumentException("Both SortKey and SortType must be provided together.");
        }

        // Parse pagination parameters (page/pagenumber and pagesize)
        if (Query.TryGetValue("page", out StringValues page))
        {
            PageNumber = int.Parse(page.First()!);
        }

        if (Query.TryGetValue("pagenumber", out StringValues pageNumber))
        {
            PageNumber = int.Parse(pageNumber.First()!);
        }

        if (Query.TryGetValue("pagesize", out StringValues pageSize))
        {
            PageSize = int.Parse(pageSize.First()!);
        }
    }
}