namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ISearchableRepository<TEntity, TKey>
{
    Task<TEntity> GetByKeyAsync(TKey key);
}

public interface IReadableRepository<TEntity, TIdentifier>
{
    Task<IEnumerable<TEntity?>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TIdentifier id);
}

public interface IWriteRepository<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
    Task DeleteAsync(TEntity entity);
}


public interface IDataRepository<TEntity, TIdentifier, TKey>
    : ISearchableRepository<TEntity, TKey>,
      IReadableRepository<TEntity, TIdentifier>,
      IWriteRepository<TEntity>
{ }