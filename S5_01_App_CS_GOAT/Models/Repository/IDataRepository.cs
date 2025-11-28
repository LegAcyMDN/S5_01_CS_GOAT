namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IReadableRepository<TEntity, TIdentifier>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(TIdentifier id);
}

public interface IWriteRepository<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity existing, TEntity updated);
    Task PatchAsync(TEntity entity, IDictionary<string, object> patchData);
    Task DeleteAsync(TEntity entity);
}

public interface IDataRepository<TEntity, TIdentifier>
    : IReadableRepository<TEntity, TIdentifier>, IWriteRepository<TEntity>
{}