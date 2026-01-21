using Microsoft.EntityFrameworkCore.ChangeTracking;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

/// <summary>
/// Provides complete CRUD operations for entities including add, update (full and partial), and delete
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TIdentifier">The type of the entity's primary key</typeparam>
/// <remarks>
/// This repository extends ReadRepository to provide write operations.
/// All operations automatically save changes to the database.
/// </remarks>
public class CrudRepository<TEntity, TIdentifier> :
    ReadRepository<TEntity, TIdentifier>,
    IDataRepository<TEntity, TIdentifier>
    where TEntity : class
    where TIdentifier : struct
{
    /// <summary>
    /// The database context used for accessing and modifying entities
    /// </summary>
    protected new readonly CSGOATDbContext _context;

    /// <summary>
    /// Initializes a new instance of the CrudRepository
    /// </summary>
    /// <param name="context">The Entity Framework database context</param>
    public CrudRepository(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Adds a new entity to the database and saves changes
    /// </summary>
    /// <param name="entity">The entity to add</param>
    /// <returns>The added entity</returns>
    /// <remarks>
    /// The entity is added to the DbSet and SaveChangesAsync is called to persist it to the database.
    /// </remarks>
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        _ = await _context.Set<TEntity>().AddAsync(entity);
        _ = await _context.SaveChangesAsync();
        return entity;
    }

    /// <summary>
    /// Updates an existing entity by attaching it to the context and saving changes
    /// </summary>
    /// <param name="entityToUpdate">The entity to update</param>
    /// <remarks>
    /// The entity is attached to the context and SaveChangesAsync is called.
    /// Any properties modified before calling this method will be persisted.
    /// </remarks>
    public async Task UpdateAsync(TEntity entityToUpdate)
    {
        _ = _context.Set<TEntity>().Attach(entityToUpdate);
        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing entity by copying values from another entity
    /// </summary>
    /// <param name="entityToUpdate">The entity to update in the database</param>
    /// <param name="entity">The entity containing the new values to copy</param>
    /// <remarks>
    /// The existing entity is attached and its CurrentValues are set to match the updated entity.
    /// All properties from the updated entity are copied to the database entity.
    /// </remarks>
    public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
    {
        _ = _context.Set<TEntity>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Partially updates an entity with only the specified properties modified
    /// </summary>
    /// <param name="entityToUpdate">The entity to patch</param>
    /// <param name="patchData">A dictionary of property names and their new values</param>
    /// <remarks>
    /// The entity is attached to the context and only the properties specified in patchData are marked as modified.
    /// This allows for PATCH HTTP requests that only update specific fields.
    /// </remarks>
    public async Task PatchAsync(TEntity entityToUpdate, IDictionary<string, object> patchData)
    {
        _ = _context.Set<TEntity>().Attach(entityToUpdate);
        EntityEntry<TEntity> entry = _context.Entry(entityToUpdate);

        // Apply each property update from the patch data
        foreach (KeyValuePair<string, object> update in patchData)
        {
            PropertyEntry property = entry.Property(update.Key);
            if (property != null)
            {
                property.CurrentValue = update.Value;
                property.IsModified = true;
            }
        }

        _ = await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity from the database
    /// </summary>
    /// <param name="entity">The entity to delete</param>
    /// <remarks>
    /// The entity is removed from the DbSet and SaveChangesAsync is called to persist the deletion.
    /// </remarks>
    public async Task DeleteAsync(TEntity entity)
    {
        _ = _context.Set<TEntity>().Remove(entity);
        _ = await _context.SaveChangesAsync();
    }
}

/// <summary>
/// Convenience implementation of CrudRepository that uses int as the primary key type
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public class CrudRepository<TEntity> : CrudRepository<TEntity, int>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the CrudRepository with int primary key
    /// </summary>
    /// <param name="context">The Entity Framework database context</param>
    public CrudRepository(CSGOATDbContext context) : base(context) { }
}