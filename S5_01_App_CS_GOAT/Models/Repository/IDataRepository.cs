using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ISearchableRepository<TEntity, TKey>
{
    Task<TEntity?> GetByKeyAsync(TKey key);
    //Task<TEntity?> GetByKeyAsync(TKey key, bool includeRelations);
}

public interface IReadableRepository<TEntity, TIdentifier>
{
    Task<IEnumerable<TEntity>> GetAllAsync(
        FilterOptions? filters = null,
        SortOptions? sorts = null);
    Task<TEntity?> GetByIdAsync(TIdentifier id);
    Task<TEntity?> GetByIdsAsync(params object[] keyValues);
}

public interface IWriteRepository<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity existing, TEntity updated);
    Task PatchAsync(TEntity entity, object patchData);
    Task DeleteAsync(TEntity entity);
}

public interface IDataRepository<TEntity, TIdentifier, TKey>
    : ISearchableRepository<TEntity, TKey>, IReadableRepository<TEntity, TIdentifier>, IWriteRepository<TEntity>
{}
