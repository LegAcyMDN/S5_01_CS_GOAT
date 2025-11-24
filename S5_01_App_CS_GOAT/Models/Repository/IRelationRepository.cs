namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IRelationRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetByRelationAsync<TRelationId>(
        TRelationId id,
        string relationName,
        FilterOptions? filters = null,
        SortOptions? sorts = null);
}
