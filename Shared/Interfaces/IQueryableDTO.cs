using Shared.Enum;

namespace Shared.Interfaces;

public interface IQueryableDTO
{
    static abstract string? DefaultSortKey { get; }
    static abstract SortingType? DefaultSortType { get; }
    static abstract int DefaultPageSize { get; }
    static abstract bool CanSearch { get; }
    string? SearchTerm { get; }
}