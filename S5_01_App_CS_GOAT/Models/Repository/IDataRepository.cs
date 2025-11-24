namespace S5_01_App_CS_GOAT.Models.Repository;

public class FilterOptions // à mettre dans un dossier service
{ 
    public Dictionary<string, object?>? Filters { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class SortOptions 
{
    public Dictionary<string, string>? Sorts { get; set; }
}

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
}

public interface IWriteRepository<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity existing, TEntity updated);
    Task PatchAsync(TEntity entity, object patchData);
    Task DeleteAsync(TEntity entity);
}

public interface IDataRepository<TEntity, TIdentifier, TKey>
    : ISearchableRepository<TEntity, TKey>,
      IReadableRepository<TEntity, TIdentifier>,
      IWriteRepository<TEntity>
{
}
