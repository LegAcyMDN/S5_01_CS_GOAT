using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using S5_01_App_CS_GOAT.Models.EntityFramework;

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
    }

    public class ReadRepository<TEntity> : ReadRepository<TEntity, int>
    where TEntity : class
    {
        public ReadRepository(CSGOATDbContext context) : base(context) { }
    }
}
