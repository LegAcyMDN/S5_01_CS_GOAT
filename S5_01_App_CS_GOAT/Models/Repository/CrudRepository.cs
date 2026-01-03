using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.Linq.Expressions;

namespace S5_01_App_CS_GOAT.Models.Repository;

public class CrudRepository<TEntity, TIdentifier> :
    ReadRepository<TEntity, TIdentifier>,
    IDataRepository<TEntity, TIdentifier>
    where TEntity : class
    where TIdentifier : struct
{
    protected readonly CSGOATDbContext _context;

    public CrudRepository(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entityToUpdate)
    {
        _context.Set<TEntity>().Attach(entityToUpdate);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
    {
        _context.Set<TEntity>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task PatchAsync(TEntity entityToUpdate, IDictionary<string, object> patchData)
    {
        _context.Set<TEntity>().Attach(entityToUpdate);
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

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}

public class CrudRepository<TEntity> : CrudRepository<TEntity, int>
    where TEntity : class
{
    public CrudRepository(CSGOATDbContext context) : base(context) { }
}