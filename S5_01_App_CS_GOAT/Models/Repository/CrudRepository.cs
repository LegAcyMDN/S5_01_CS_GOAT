using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOAT.Models.Repository;

public abstract class CrudRepository<TEntity> : IDataRepository<TEntity, int, string> where TEntity : class
{
    protected readonly CSGOATDbContext _context;

    public CrudRepository(CSGOATDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(FilterOptions? filters = null, SortOptions? sorts = null)
    {
        // Currently ignoring filters/sorts and returning all entities; implement filtering/sorting later if needed.
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetByIdsAsync(params object[] keyValues)
    {
        return await _context.Set<TEntity>().FindAsync(keyValues);
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }


    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entityToUpdate, TEntity entity)
    {
        _context.Set<TEntity>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task PatchAsync(TEntity entityToUpdate, object patchData)
    {
        if (patchData is not IDictionary<string, object> updates)
        {
            throw new ArgumentException("patchData must be an IDictionary<string, object>", nameof(patchData));
        }

        _context.Set<TEntity>().Attach(entityToUpdate);
        EntityEntry<TEntity> entry = _context.Entry(entityToUpdate);

        foreach (KeyValuePair<string, object> update in updates)
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