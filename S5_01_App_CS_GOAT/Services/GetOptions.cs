using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Shared.Enum;
using Shared.Interfaces;
using System.Reflection;
using System.Text.Json.Serialization;

namespace S5_01_App_CS_GOAT.Services;

public class GetOptions<Tdto>
    where Tdto : class, IQueryableDTO
{
    private static readonly Dictionary<List<string>, SortingType> SortingTypeMapping = new()
    {
        { new() { "a", "asc", "ascend", "ascending" }, SortingType.Ascending },
        { new() { "d", "des", "desc", "descend", "descending" }, SortingType.Descending },
        { new() { "s", "search", "levenshtein" }, SortingType.Search }
    };

    private static BindingFlags BindingFlags = 
        BindingFlags.IgnoreCase |
        BindingFlags.Public |
        BindingFlags.Instance;

    private static List<string> KeyWords = new List<string>()
    {
        "sorttype", "sortkey", "page", "pagenumber", "pagesize"
    };


    private IQueryCollection? _query;
    private string? _sortKey;
    private SortingType? _sortType;
    private int _pageNumber = 1;
    private int? _pageSize;


    public List<string> PropertyNames =>
        typeof(Tdto).GetProperties(BindingFlags).Select(p => p.Name).ToList();

    private IQueryCollection? Query
    {
        get => _query;
        set
        {
            _query = value;
            ParseQuery();
        }
    }

    public IEnumerable<Tdto> Result { get; set; } = Enumerable.Empty<Tdto>();

    public Dictionary<string, List<string?>> Filters
    {
        get;
        private set;
    } = new Dictionary<string, List<string?>>();
    
    public string? SortKey
    {
        get => _sortKey ?? Tdto.DefaultSortKey;
        set {
            if (SortType != SortingType.Search && 
                !PropertyNames.Any(p => p.Equals(value, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Invalid sort key: {value}");
            _sortKey = value;
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SortingType? SortType
    {
        get => _sortType ?? Tdto.DefaultSortType;
        set
        {
            if (value == SortingType.Search && !CanSearch)
                throw new ArgumentException("Search sorting type is not supported for this DTO.");
            _sortType = value;
        }
    }

    public int PageNumber
    {
        get => _pageNumber;
        set {
            if (value <= 0)
                throw new ArgumentException("PageNumber must be greater than zero.");
            _pageNumber = value;
        }
    }

    public int PageSize
    {
        get => _pageSize ?? Tdto.DefaultPageSize;
        set {
            if (value <= 0)
                throw new ArgumentException("PageSize must be greater than zero.");
            _pageSize = value;
        }
    }

    public int Count => Result.Count();
    public int PageCount => (int)Math.Ceiling((double)TotalCount / PageSize);
    public int FilteredCount { get; private set; } = 0;
    public int TotalCount { get; private set; } = 0;
    public bool CanSearch => Tdto.CanSearch;
    public string DtoTypeName => typeof(Tdto).Name;


    public IEnumerable<Tdto> ApplyFilters(IEnumerable<Tdto> result)
    {
        if (!result.Any()) return result;
        foreach (var filter in Filters)
        {
            result = result.Where(item =>
            {
                string? propertyValue = item.GetType().GetProperty(filter.Key, BindingFlags)?.GetValue(item, null)?.ToString();
                return filter.Value.Contains(propertyValue);
            });
        }
        return result;
    }

    public IEnumerable<Tdto> ApplySorting(IEnumerable<Tdto> result)
    {
        if (!result.Any()) return result;
        if (SortKey == null) return result;
        switch (SortType ?? SortingType.Ascending)
        {
            case SortingType.Ascending:
                return result.OrderBy(item => item.GetType().GetProperty(SortKey, BindingFlags)?.GetValue(item, null));
            case SortingType.Descending:
                return result.OrderByDescending(item => item.GetType().GetProperty(SortKey, BindingFlags)?.GetValue(item, null));
            case SortingType.Search:
                return Search(result, SortKey);
            default: throw new Exception(); // Unreachable
        }
    }

    public IEnumerable<Tdto> ApplyPaging(IEnumerable<Tdto> result)
    {
        return result.Skip((PageNumber - 1) * PageSize).Take(PageSize);
    }

    private GetOptions<Tdto> Apply()
    {
        if (Result == null || Count == 0) return this;
        TotalCount = Result.Count();
        Result = ApplyFilters(Result);
        FilteredCount = Result.Count();
        Result = ApplySorting(Result);
        Result = ApplyPaging(Result);
        return this;
    }

    public GetOptions<Tdto> Apply(IEnumerable<Tdto>? result)
    {
        if (result == null) return this;
        Result = result;
        Apply();
        return this;
    }

    public static IEnumerable<Tdto> Search(IEnumerable<Tdto> result, string searchTerm)
    {
        if (!result.Any()) return result;
        if (!Tdto.CanSearch) return result;
        if (string.IsNullOrWhiteSpace(searchTerm)) return result;

        result = result.OrderBy(item => ComputeLevenshteinDistance(
            item.SearchTerm?.ToLower() ?? "",
            searchTerm.ToLower()
        ));
        return result;
    }

    private static int ComputeLevenshteinDistance(string source, string target)
    {
        if (string.IsNullOrEmpty(source))
            return string.IsNullOrEmpty(target) ? 0 : target.Length;
        
        if (string.IsNullOrEmpty(target))
            return source.Length;

        int sourceLength = source.Length;
        int targetLength = target.Length;

        int[,] distance = new int[sourceLength + 1, targetLength + 1];

        for (int i = 0; i <= sourceLength; i++)
            distance[i, 0] = i;
        
        for (int j = 0; j <= targetLength; j++)
            distance[0, j] = j;

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


    public GetOptions() { }

    public GetOptions(IQueryCollection? query, IEnumerable<Tdto>? result)
        { ParseQuery(query); Apply(result); }

    public GetOptions(HttpRequest? request, IEnumerable<Tdto>? result)
        : this(request?.Query, result) { }

    public GetOptions(IQueryCollection? query)
        : this(query, null) { }

    public GetOptions(HttpRequest? request)
        : this(request, null) { }

    public GetOptions(IEnumerable<Tdto>? result)
        : this(query: null, result) { }
    

    public void ParseQuery(HttpRequest request)
        { ParseQuery(request.Query); }

    public void ParseQuery(IQueryCollection? query = null)
    {
        if (query != null) Query = query;
        if (Query == null) return;

        // Parse filters
        foreach (var item in Query)
        {
            string key = item.Key.ToLower();
            if (KeyWords.Contains(key)) continue;
            if (!PropertyNames.Any(p => p.Equals(key, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException($"Invalid filter key: {key}");
            Filters[key] = item.Value.ToList();
        }

        // Parse sorting
        if (Query.TryGetValue("sorttype", out var sortType))
            SortType = SortingTypeMapping.SelectMany(kv => kv.Key
                .Where(s => s.Equals(sortType.First(), StringComparison.OrdinalIgnoreCase))
                .Select(_ => kv.Value))
                .First();
        if (Query.TryGetValue("sortkey", out var sortKey))
            SortKey = sortKey.First()!;
        
        if ((SortType != null ? 1: 0) + (SortKey != null ? 1 : 0) == 1)
            throw new ArgumentException("Both SortKey and SortType must be provided together.");

        // Parse pagination
        if (Query.TryGetValue("page", out var page))
            PageNumber = int.Parse(page.First()!);
        if (Query.TryGetValue("pagenumber", out var pageNumber))
            PageNumber = int.Parse(pageNumber.First()!);

        if (Query.TryGetValue("pagesize", out var pageSize))
            PageSize = int.Parse(pageSize.First()!);
    }
}