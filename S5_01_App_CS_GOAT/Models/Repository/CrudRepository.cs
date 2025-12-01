using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.Linq.Expressions;

namespace S5_01_App_CS_GOAT.Models.Repository;

public class CrudRepository<TEntity, TIdentifier> :
    IDataRepository<TEntity, TIdentifier>
    where TEntity : class
    where TIdentifier : struct
{
    protected readonly CSGOATDbContext _context;

    public CrudRepository(CSGOATDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(
        Expression<Func<TEntity, bool>>? where = null,
        params string[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();
        foreach (var include in includes)
            query = query.Include(include);
        if (where != null) query = query.Where(where);
        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TIdentifier id)
    {
        if (typeof(TIdentifier).Name.StartsWith("ValueTuple"))
        {
            var fields = typeof(TIdentifier).GetFields();
            var values = new object[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                values[i] = fields[i].GetValue(id)!;
            }
            return await _context.Set<TEntity>().FindAsync(values);
        }
        
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity?> GetByIdAsync(int id, params string[] includes)
    {
        IQueryable<TEntity> query = _context.Set<TEntity>();
        foreach (var include in includes)
            query = query.Include(include);
        string? keyName = _context.Model
            .FindEntityType(typeof(TEntity))?
            .FindPrimaryKey()?
            .Properties
            .Select(x => x.Name)
            .FirstOrDefault();
        if (keyName == null) throw new InvalidOperationException(
            $"Entity {typeof(TEntity).Name} does not have a primary key defined.");
        return await query.FirstOrDefaultAsync(
            e => EF.Property<int>(e, keyName) == id);
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