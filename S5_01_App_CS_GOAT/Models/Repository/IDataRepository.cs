using S5_01_App_CS_GOAT.Services;
using System.Linq.Expressions;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IReadableRepository<TEntity, TIdentifier> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options);

    Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? where = null,
        params string[] includes
        );
    Task<TEntity?> GetByIdAsync(int id,
        params string[] includes);

    Task<TEntity?> GetByIdAsync(TIdentifier id, QueryOptions<TEntity>? options = null);
    Task<TEntity?> GetByIdAsync(bool LUL, TIdentifier id, QueryOptions<TEntity>? options = null);
}

public interface IWriteRepository<TEntity>
{
    Task<TEntity> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity existing);
    Task UpdateAsync(TEntity existing, TEntity updated);
    Task PatchAsync(TEntity entity, IDictionary<string, object> patchData);
    Task DeleteAsync(TEntity entity);
}

public interface IDataRepository<TEntity, TIdentifier>
    : IReadableRepository<TEntity, TIdentifier>, IWriteRepository<TEntity>
    where TEntity : class
{ }

public interface ITypeRepository<TEntity> :
    IReadableRepository<TEntity, int>
    where TEntity : class
{
    TEntity? GetTypeByName(string typeName);
}