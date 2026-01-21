using Microsoft.EntityFrameworkCore.ChangeTracking;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

public class CrudRepository<TEntity, TIdentifier> :
    ReadRepository<TEntity, TIdentifier>,
    IDataRepository<TEntity, TIdentifier>
    where TEntity : class
    where TIdentifier : struct
{
    protected new readonly CSGOATDbContext _context;

    public CrudRepository(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        _ = await _context.Set<TEntity>().AddAsync(entity);
        _ = await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entityToUpdate)
    {
        _ = _context.Set<TEntity>().Attach(entityToUpdate);
        _ = await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
    {
        _ = _context.Set<TEntity>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        _ = await _context.SaveChangesAsync();
    }

    public async Task PatchAsync(TEntity entityToUpdate, IDictionary<string, object> patchData)
    {
        _ = _context.Set<TEntity>().Attach(entityToUpdate);
        EntityEntry<TEntity> entry = _context.Entry(entityToUpdate);

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

    public async Task DeleteAsync(TEntity entity)
    {
        _ = _context.Set<TEntity>().Remove(entity);
        _ = await _context.SaveChangesAsync();
    }
}

public class CrudRepository<TEntity> : CrudRepository<TEntity, int>
    where TEntity : class
{
    public CrudRepository(CSGOATDbContext context) : base(context) { }
}