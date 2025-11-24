namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IHistoryRepository<TEntity, TRelatedId>
{
    Task<IEnumerable<TEntity>> GetByRelatedAsync(
        TRelatedId relatedId,
        FilterOptions? filters = null,
        SortOptions? sorts = null);
}
