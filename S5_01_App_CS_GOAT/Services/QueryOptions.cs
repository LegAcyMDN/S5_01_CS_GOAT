namespace S5_01_App_CS_GOAT.Services;

public interface IQueryableEntity
{
    public int? Id { get; set; }

    public string? Key { get; set; }

    public double? Value { get; set; }

    public string? Category { get; set; }

    public DateTime? Date { get; set; }
}

public interface IQueryOption
{
    public IEnumerable<T> Apply<T>(IEnumerable<T> collection)
        where T : IQueryableEntity;
}

public class QueryOptions : IQueryOption
{
    public SortOptions SortOptions { get; set; } = new SortOptions();

    public FilterOptions FilterOptions { get; set; } = new FilterOptions();

    public PageOptions PageOptions { get; set; } = new PageOptions();

    public IEnumerable<T> Apply<T>(IEnumerable<T> collection)
        where T : IQueryableEntity
    {
        collection = SortOptions.Apply(collection);
        collection = FilterOptions.Apply(collection);
        collection = PageOptions.Apply(collection);
        return collection;
    }
}

public enum SortDirection
{
    None,
    Ascending,
    Descending
}

public class SortOptions : IQueryOption
{
    public SortDirection SortById { get; set; } = SortDirection.Ascending;

    public SortDirection SortByKey { get; set; } = SortDirection.None;

    public SortDirection SortByValue { get; set; } = SortDirection.None;

    public SortDirection SortByCategory { get; set; } = SortDirection.None;

    public SortDirection SortByDate { get; set; } = SortDirection.None;

    public IEnumerable<T> Apply<T>(IEnumerable<T> collection)
        where T : IQueryableEntity
    {
        if (SortById != SortDirection.None)
        {
            collection = SortById == SortDirection.Ascending
                ? collection.OrderBy(e => e.Id)
                : collection.OrderByDescending(e => e.Id);
        }

        if (SortByKey != SortDirection.None)
        {
            collection = SortByKey == SortDirection.Ascending
                ? collection.OrderBy(e => e.Key)
                : collection.OrderByDescending(e => e.Key);
        }

        if (SortByValue != SortDirection.None)
        {
            collection = SortByValue == SortDirection.Ascending
                ? collection.OrderBy(e => e.Value)
                : collection.OrderByDescending(e => e.Value);
        }

        if (SortByCategory != SortDirection.None)
        {
            collection = SortByCategory == SortDirection.Ascending
                ? collection.OrderBy(e => e.Category)
                : collection.OrderByDescending(e => e.Category);
        }

        if (SortByDate != SortDirection.None)
        {
            collection = SortByDate == SortDirection.Ascending
                ? collection.OrderBy(e => e.Date)
                : collection.OrderByDescending(e => e.Date);
        }

        return collection;
    }
}

public class FilterOptions : IQueryOption
{
    public string? KeyFilter { get; set; }

    public int MaxDistanceFromKey { get; set; } = 5;

    public bool SortByKeyDistance { get; set; } = true;

    public double? MinPriceFilter { get; set; }

    public double? MaxPriceFilter { get; set; }

    public string[] CategoryFilters { get; set; } = Array.Empty<string>();

    public DateTime? MinDateFilter { get; set; }

    public DateTime? MaxDateFilter { get; set; }

    public IEnumerable<T> Apply<T>(IEnumerable<T> collection)
        where T : IQueryableEntity
    {
        if (!string.IsNullOrEmpty(KeyFilter))
        {
            throw new NotImplementedException("Key-based filtering is not implemented yet.");
        }

        if (MinPriceFilter.HasValue)
        {
            collection = collection
                .Where(e => e.Value.HasValue && e.Value.Value >= MinPriceFilter.Value);
        }

        if (MaxPriceFilter.HasValue)
        {
            collection = collection
                .Where(e => e.Value.HasValue && e.Value.Value <= MaxPriceFilter.Value);
        }

        if (CategoryFilters.Length > 0)
        {
            collection = collection
                .Where(e => e.Category != null && CategoryFilters.Contains(e.Category));
        }

        if (MinDateFilter.HasValue)
        {
            collection = collection
                .Where(e => e.Date.HasValue && e.Date.Value >= MinDateFilter.Value);
        }

        if (MaxDateFilter.HasValue)
        {
            collection = collection
                .Where(e => e.Date.HasValue && e.Date.Value <= MaxDateFilter.Value);
        }

        return collection;
    }
}

public class PageOptions : IQueryOption
{
    public int Page { get; set; } = 1;
    
    public int PageSize { get; set; } = int.MaxValue;

    public IEnumerable<T> Apply<T>(IEnumerable<T> collection)
        where T : IQueryableEntity
    {
        return collection
            .Skip((Page - 1) * PageSize)
            .Take(PageSize);
    }
}