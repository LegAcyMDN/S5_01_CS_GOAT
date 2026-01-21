using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

/// <summary>
/// Defines read-only operations for accessing entities from the database
/// </summary>
/// <typeparam name="TEntity">The entity type being queried</typeparam>
/// <typeparam name="TIdentifier">The type of the entity's primary key</typeparam>
public interface IReadableRepository<TEntity, TIdentifier> where TEntity : class
{
    /// <summary>
    /// Retrieves all entities from the database with optional query options
    /// </summary>
    /// <param name="options">Optional QueryOptions for filtering and eager loading</param>
    /// <returns>An enumerable collection of all entities</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null);

    /// <summary>
    /// Retrieves a specific entity by its primary key with optional query options
    /// </summary>
    /// <param name="id">The primary key value of the entity to retrieve</param>
    /// <param name="options">Optional QueryOptions for filtering and eager loading</param>
    /// <returns>The entity if found; otherwise null</returns>
    Task<TEntity?> GetByIdAsync(TIdentifier id, QueryOptions<TEntity>? options = null);
}

/// <summary>
/// Defines write operations for modifying entities in the database
/// </summary>
/// <typeparam name="TEntity">The entity type being modified</typeparam>
public interface IWriteRepository<TEntity>
{
    /// <summary>
    /// Adds a new entity to the database and saves changes
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>The added entity</returns>
    Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Updates an existing entity by attaching it and saving changes
    /// </summary>
    /// <param name="existing">The entity to update</param>
    Task UpdateAsync(TEntity existing);

    /// <summary>
    /// Updates an existing entity with values from another entity
    /// </summary>
    /// <param name="existing">The entity to update in the database</param>
    /// <param name="updated">The entity containing the new values</param>
    Task UpdateAsync(TEntity existing, TEntity updated);

    /// <summary>
    /// Partially updates an entity with only the specified properties modified
    /// </summary>
    /// <param name="entity">The entity to patch</param>
    /// <param name="patchData">A dictionary of property names and their new values</param>
    Task PatchAsync(TEntity entity, IDictionary<string, object> patchData);

    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    Task DeleteAsync(TEntity entity);
}

/// <summary>
/// Combines read and write operations for complete CRUD functionality
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TIdentifier">The type of the entity's primary key</typeparam>
public interface IDataRepository<TEntity, TIdentifier>
    : IReadableRepository<TEntity, TIdentifier>, IWriteRepository<TEntity>
    where TEntity : class
{ }

/// <summary>
/// Provides read-only operations for type/lookup entities
/// </summary>
/// <typeparam name="TEntity">The type entity being queried</typeparam>
public interface ITypeRepository<TEntity> :
    IReadableRepository<TEntity, int>
    where TEntity : class
{
    /// <summary>
    /// Retrieves a type entity by its name
    /// </summary>
    /// <param name="typeName">The name of the type to retrieve</param>
    /// <returns>The type entity if found; otherwise null</returns>
    TEntity? GetTypeByName(string typeName);
}