using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Provides read-only access to entities in the database with support for query options
    /// </summary>
    /// <typeparam name="TEntity">The entity type to query</typeparam>
    /// <typeparam name="TIdentifier">The type of the entity's primary key</typeparam>
    /// <remarks>
    /// This repository applies QueryOptions to support eager loading and filtering both at the database level (before)
    /// and client-side (after). It handles both simple primary keys and composite keys using ValueTuple.
    /// </remarks>
    public class ReadRepository<TEntity, TIdentifier> :
        IReadableRepository<TEntity, TIdentifier>
    where TEntity : class
    where TIdentifier : struct
    {

        /// <summary>
        /// The database context used for accessing entities
        /// </summary>
        protected readonly CSGOATDbContext _context;

        /// <summary>
        /// Initializes a new instance of the ReadRepository
        /// </summary>
        /// <param name="context">The Entity Framework database context</param>
        public ReadRepository(CSGOATDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all entities from the database with optional query options
        /// </summary>
        /// <param name="options">Optional QueryOptions for eager loading and filtering</param>
        /// <returns>An enumerable collection of all entities from the database</returns>
        /// <remarks>
        /// The QueryOptions are applied in two phases:
        /// 1. "Before" options are applied to the IQueryable before executing the database query
        /// 2. "After" options are applied to the in-memory collection after the query executes
        /// </remarks>
        public async Task<IEnumerable<TEntity>> GetAllAsync(QueryOptions<TEntity>? options = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            // Apply database-side options (includes, filters that should be in WHERE clause)
            if (options != null)
            {
                query = options.ApplyBefore(query);
            }

            // Execute the query and retrieve results
            IEnumerable<TEntity> list = await query.ToListAsync();
            // Apply client-side options (lazy-loaded navigation properties, client-side filters)
            if (options != null)
            {
                _ = await options.ApplyAfter(list, _context);
            }

            return list;
        }

        /// <summary>
        /// Retrieves a specific entity by its primary key with optional query options
        /// </summary>
        /// <param name="id">The primary key value of the entity to retrieve</param>
        /// <param name="options">Optional QueryOptions for eager loading and filtering</param>
        /// <returns>The entity if found; otherwise null</returns>
        /// <remarks>
        /// This method supports both simple primary keys and composite primary keys (ValueTuple).
        /// It retrieves the primary key property name(s) from the Entity Framework model metadata.
        /// </remarks>
        public async Task<TEntity?> GetByIdAsync(TIdentifier id, QueryOptions<TEntity>? options = null)
        {
            DbSet<TEntity> dbSet = _context.Set<TEntity>();
            IQueryable<TEntity> query = dbSet.AsQueryable();

            // Apply database-side options (eager loading and filtering)
            if (options != null)
            {
                query = options.ApplyBefore(query);
            }
            TEntity? entity;

            // Handle composite primary keys (ValueTuple)
            if (typeof(TIdentifier).Name.StartsWith("ValueTuple"))
            {
                System.Reflection.FieldInfo[] fields = typeof(TIdentifier).GetFields();
                object[] values = new object[fields.Length];
                for (int i = 0; i < fields.Length; i++)
                {
                    values[i] = fields[i].GetValue(id)!;
                }
                // Query by the first field of the composite key
                entity = await query.FirstOrDefaultAsync(e => EF.Property<object>(e, fields[0].Name).Equals(values[0]));
            }
            else
            {
                // Handle simple primary keys - retrieve the primary key property name from the model
                string? keyName = (_context.Model
                    .FindEntityType(typeof(TEntity))?
                    .FindPrimaryKey()?
                    .Properties
                    .Select(x => x.Name)
                    .FirstOrDefault()) ?? throw new InvalidOperationException(
                        $"Entity {typeof(TEntity).Name} does not have a primary key defined.");
                // Query by the primary key
                entity = await query.FirstOrDefaultAsync(e => EF.Property<TIdentifier>(e, keyName).Equals(id));
            }

            // Apply client-side options if entity was found
            if (entity != null && options != null)
            {
                entity = await options.ApplyAfter(entity, _context);
            }

            return entity;
        }
    }

    /// <summary>
    /// Convenience implementation of ReadRepository that uses int as the primary key type
    /// </summary>
    /// <typeparam name="TEntity">The entity type to query</typeparam>
    public class ReadRepository<TEntity> : ReadRepository<TEntity, int>
        where TEntity : class
    {
        /// <summary>
        /// Initializes a new instance of the ReadRepository with int primary key
        /// </summary>
        /// <param name="context">The Entity Framework database context</param>
        public ReadRepository(CSGOATDbContext context) : base(context) { }
    }
}
