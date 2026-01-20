using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public class ReadRepository<TEntity, TIdentifier> :
        IReadableRepository<TEntity, TIdentifier>
    where TEntity : class
    where TIdentifier : struct
    {

        protected readonly CSGOATDbContext _context;

        public ReadRepository(CSGOATDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            if (options != null) query = options.ApplyBefore(query);
            IEnumerable<TEntity> list = await query.ToListAsync();
            if (options != null) await options.ApplyAfter(list, _context);
            return list;
        }

        public async Task<TEntity?> GetByIdAsync(TIdentifier id, QueryOptions<TEntity>? options = null)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>(); // Use DbSet for FindAsync
            IQueryable<TEntity> query = dbSet.AsQueryable(); // Ensure query is IQueryable

            if (options != null)
            {
                query = options.ApplyBefore(query);
            }
            TEntity? entity;

            if (typeof(TIdentifier).Name.StartsWith("ValueTuple"))
            {
                var fields = typeof(TIdentifier).GetFields();
                var values = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    values[i] = fields[i].GetValue(id)!;
                }
                entity = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, fields[0].Name).Equals(values[0]));
            }
            else
            {
                string? keyName = _context.Model
                    .FindEntityType(typeof(TEntity))?
                    .FindPrimaryKey()?
                    .Properties
                    .Select(x => x.Name)
                    .FirstOrDefault();
                if (keyName == null) throw new InvalidOperationException(
                    $"Entity {typeof(TEntity).Name} does not have a primary key defined.");
                entity = await query.FirstOrDefaultAsync(e => EF.Property<TIdentifier>(e, keyName).Equals(id));
            }

            if (entity != null && options != null)
            {
                entity = await options.ApplyAfter(entity, _context);
            }

            return entity;
        }
    }

    public class ReadRepository<TEntity> : ReadRepository<TEntity, int>
        where TEntity : class
    {
        public ReadRepository(CSGOATDbContext context) : base(context) { }
    }
}
