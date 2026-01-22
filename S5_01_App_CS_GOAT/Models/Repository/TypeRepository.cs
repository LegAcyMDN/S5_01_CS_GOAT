using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

/// <summary>
/// Provides read-only access to type/lookup entities with specialized querying by name
/// </summary>
/// <typeparam name="TEntity">The type entity being queried, must implement IType interface</typeparam>
/// <remarks>
/// This repository is specifically designed for entities that represent types or lookups (e.g., categories, statuses).
/// It extends ReadRepository with an additional method to retrieve types by their name property.
/// </remarks>
public class TypeRepository<TEntity> :
    ReadRepository<TEntity, int>,
    ITypeRepository<TEntity>
    where TEntity : class, IType
{
    /// <summary>
    /// The database context used for accessing type entities
    /// </summary>
    private new readonly CSGOATDbContext _context;

    /// <summary>
    /// Initializes a new instance of the TypeRepository
    /// </summary>
    /// <param name="context">The Entity Framework database context</param>
    public TypeRepository(CSGOATDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves a type entity by its name
    /// </summary>
    /// <param name="typeName">The name of the type to retrieve</param>
    /// <returns>The type entity if found; otherwise null</returns>
    /// <remarks>
    /// This method retrieves all types from the database and filters them in memory by name.
    /// The comparison is case-sensitive. For large datasets, consider implementing a database-side query.
    /// </remarks>
    public TEntity? GetTypeByName(string typeName)
    {
        // Retrieve all types and search by name (case-sensitive)
        return _context.Set<TEntity>().ToList()
            .Where(t => t.TypeName == typeName)
            .FirstOrDefault();
    }
}