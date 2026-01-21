using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace S5_01_App_CS_GOAT.Services;

/// <summary>
/// Represents a single query option that can apply filtering, eager loading, or navigation property loading to Entity Framework queries
/// </summary>
/// <typeparam name="TEntity">The entity type to query</typeparam>
public class QueryOption<TEntity> where TEntity : class
{
    private readonly Expression<Func<TEntity, bool>>? Where;
    private readonly Expression<Func<TEntity, object?>>? Include;
    private readonly string? Path;

    /// <summary>
    /// Initializes a new instance with optional where clause, include expression, and include path
    /// </summary>
    public QueryOption(Expression<Func<TEntity, bool>>? where, Expression<Func<TEntity, object?>>? include, string? includePath)
    {
        Where = where;
        Include = include;
        Path = includePath;
    }

    /// <summary>
    /// Initializes a new instance with a where clause for filtering
    /// </summary>
    public QueryOption(Expression<Func<TEntity, bool>> where)
        : this(where, null, null) { }

    /// <summary>
    /// Initializes a new instance with an include expression for eager loading
    /// </summary>
    public QueryOption(Expression<Func<TEntity, object?>> include)
        : this(null, include, null) { }

    /// <summary>
    /// Initializes a new instance with a string path for eager loading nested navigation properties
    /// </summary>
    public QueryOption(string include)
        : this(null, null, include) { }

    /// <summary>
    /// Applies this query option to an IQueryable for database queries
    /// </summary>
    /// <param name="query">The queryable to modify</param>
    /// <returns>The modified queryable with includes and filters applied</returns>
    public IQueryable<TEntity> ApplyOption(IQueryable<TEntity> query)
    {
        // Apply eager loading via Include expression
        if (Include != null)
        {
            query = query.Include(Include);
        }
        // Apply eager loading via string path
        if (Path != null)
        {
            query = query.Include(Path);
        }
        // Apply filtering
        if (Where != null)
        {
            query = query.Where(Where);
        }
        return query;
    }

    /// <summary>
    /// Applies this query option to an in-memory collection by explicitly loading navigation properties
    /// </summary>
    /// <param name="collection">The collection to process</param>
    /// <param name="context">The database context for loading navigation properties</param>
    /// <returns>The processed collection with navigation properties loaded and filters applied</returns>
    public async Task<IEnumerable<TEntity>> ApplyOption(IEnumerable<TEntity> collection, DbContext context)
    {
        // Load navigation properties from Include expression
        if (Include != null && collection.Any())
        {
            List<string> path = GetNavigation(Include);
            await LoadNavigation(collection, context, path);
        }
        // Load navigation properties from string path
        if (Path != null && collection.Any())
        {
            string[] segments = Path.Split('.');
            List<string> path = segments.ToList();
            await LoadNavigation(collection, context, path);
        }
        // Apply filtering
        if (Where != null)
        {
            collection = collection.AsQueryable().Where(Where);
        }
        return collection;
    }

    /// <summary>
    /// Applies this query option to a single entity by explicitly loading its navigation properties
    /// </summary>
    /// <param name="entity">The entity to process</param>
    /// <param name="context">The database context for loading navigation properties</param>
    /// <returns>The entity with navigation properties loaded</returns>
    public async Task<TEntity> ApplyOption(TEntity entity, DbContext context)
    {
        if (Include != null)
        {
            List<string> path = GetNavigation(Include);
            await LoadNavigation(entity, context, path);
        }
        if (Path != null)
        {
            string[] segments = Path.Split('.');
            List<string> path = segments.ToList();
            await LoadNavigation(entity, context, path);
        }
        return entity;
    }

    /// <summary>
    /// Extracts the navigation property path from a lambda expression
    /// </summary>
    /// <param name="include">The lambda expression representing the navigation path</param>
    /// <returns>A list of property names in the navigation path</returns>
    /// <exception cref="ArgumentException">Thrown when the expression is not a valid member access expression</exception>
    private static List<string> GetNavigation(Expression<Func<TEntity, object?>> include)
    {
        List<string> path = [];
        Expression current = include.Body;

        // Convert unary expressions (e.g., Convert operations) to member expressions
        if (current is UnaryExpression unaryExpr &&
            unaryExpr.NodeType == ExpressionType.Convert)
        {
            current = unaryExpr.Operand;
        }

        // Walk the expression tree to build the navigation path
        while (current is MemberExpression memberExpr)
        {
            path.Insert(0, memberExpr.Member.Name);
            current = memberExpr.Expression!;
        }

        return path.Count == 0
            ? throw new ArgumentException($"{include} is invalid. Must be a member access expression.", nameof(include))
            : path;
    }

    /// <summary>
    /// Loads navigation properties for all entities in a collection
    /// </summary>
    private async Task LoadNavigation<T>(IEnumerable<T> collection, DbContext context, List<string> path)
    {
        foreach (T entity in collection)
        {
            await LoadNavigation(entity, context, path);
        }
    }

    /// <summary>
    /// Recursively loads navigation properties for a single entity following the specified path
    /// </summary>
    private async Task LoadNavigation<T>(T? entity, DbContext context, List<string> path)
    {
        if (entity == null)
        {
            return;
        }

        string propertyName = path[0];
        var remainingPath = path.Skip(1).ToList();

        EntityEntry entry = context.Entry(entity);
        NavigationEntry navigation = entry.Navigation(propertyName);
        // Load the navigation property if not already loaded
        if (navigation.IsLoaded == false)
        {
            await navigation.LoadAsync();
        }

        // If this is the last property in the path, we're done
        if (remainingPath.Count == 0)
        {
            return;
        }

        // Recursively load nested navigation properties
        if (navigation.Metadata.IsCollection)
        {
            var relatedEntities = navigation.CurrentValue as IEnumerable<object?>;
            if (relatedEntities == null)
            {
                return;
            }

            foreach (object? relatedEntity in relatedEntities)
            {
                if (relatedEntity == null)
                {
                    continue;
                }

                await LoadNavigation(relatedEntity, context, remainingPath);
            }
        }
        else
        {
            object? relatedEntity = navigation.CurrentValue;
            if (relatedEntity == null)
            {
                return;
            }

            await LoadNavigation(relatedEntity, context, remainingPath);
        }
    }
}

/// <summary>
/// Provides a fluent interface for building complex Entity Framework queries with before/after options and sorting
/// </summary>
/// <typeparam name="TEntity">The entity type to query</typeparam>
/// <remarks>
/// Before options are applied to IQueryable (database-side), while After options are applied to results (client-side).
/// This allows for efficient querying while still supporting complex navigation property loading.
/// </remarks>
public class QueryOptions<TEntity> where TEntity : class
{
    /// <summary>Options applied before query execution (database-side)</summary>
    private readonly List<QueryOption<TEntity>> BeforeOptions = [];
    /// <summary>Options applied after query execution (client-side)</summary>
    private readonly List<QueryOption<TEntity>> AfterOptions = [];
    /// <summary>The property to sort by</summary>
    private Expression<Func<TEntity, object?>>? Sorting;
    /// <summary>Whether to sort in descending order</summary>
    private bool SortIsDescending = false;

    /// <summary>
    /// Initializes a new instance of the QueryOptions class
    /// </summary>
    public QueryOptions() { }

    /// <summary>
    /// Adds query options to be applied before query execution (database-side)
    /// </summary>
    /// <param name="options">The query options to add</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> Before(params QueryOption<TEntity>[] options)
    {
        BeforeOptions.AddRange(options);
        return this;
    }

    /// <summary>
    /// Adds where clauses to be applied before query execution (database-side filtering)
    /// </summary>
    /// <param name="wheres">The where clause expressions</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> Before(params Expression<Func<TEntity, bool>>[] wheres)
    {
        foreach (Expression<Func<TEntity, bool>> where in wheres)
        {
            BeforeOptions.Add(new QueryOption<TEntity>(where));
        }
        return this;
    }

    /// <summary>
    /// Adds include expressions for eager loading before query execution (database-side joins)
    /// </summary>
    /// <param name="includes">The include expressions for navigation properties</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> Before(params Expression<Func<TEntity, object?>>[] includes)
    {
        foreach (Expression<Func<TEntity, object?>> include in includes)
        {
            BeforeOptions.Add(new QueryOption<TEntity>(include));
        }
        return this;
    }

    /// <summary>
    /// Adds string paths for eager loading before query execution (database-side joins)
    /// </summary>
    /// <param name="paths">The navigation property paths (e.g., "Author.Books")</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> Before(params string[] paths)
    {
        foreach (string path in paths)
        {
            BeforeOptions.Add(new QueryOption<TEntity>(path));
        }
        return this;
    }

    /// <summary>
    /// Adds query options to be applied after query execution (client-side)
    /// </summary>
    /// <param name="options">The query options to add</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> After(params QueryOption<TEntity>[] options)
    {
        AfterOptions.AddRange(options);
        return this;
    }

    /// <summary>
    /// Adds where clauses to be applied after query execution (client-side filtering)
    /// </summary>
    /// <param name="wheres">The where clause expressions</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> After(params Expression<Func<TEntity, bool>>[] wheres)
    {
        foreach (Expression<Func<TEntity, bool>> where in wheres)
        {
            AfterOptions.Add(new QueryOption<TEntity>(where));
        }
        return this;
    }

    /// <summary>
    /// Adds include expressions for explicit loading after query execution (client-side navigation property loading)
    /// </summary>
    /// <param name="includes">The include expressions for navigation properties</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> After(params Expression<Func<TEntity, object?>>[] includes)
    {
        foreach (Expression<Func<TEntity, object?>> include in includes)
        {
            AfterOptions.Add(new QueryOption<TEntity>(include));
        }
        return this;
    }

    /// <summary>
    /// Adds string paths for explicit loading after query execution (client-side navigation property loading)
    /// </summary>
    /// <param name="paths">The navigation property paths (e.g., "Author.Books")</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> After(params string[] paths)
    {
        foreach (string path in paths)
        {
            AfterOptions.Add(new QueryOption<TEntity>(path));
        }
        return this;
    }

    /// <summary>
    /// Sets the sorting property and direction
    /// </summary>
    /// <param name="property">The property to sort by</param>
    /// <param name="isDescending">Whether to sort in descending order (default: false)</param>
    /// <returns>This QueryOptions instance for method chaining</returns>
    public QueryOptions<TEntity> OrderBy(
        Expression<Func<TEntity, object?>> property,
        bool isDescending = false)
    {
        Sorting = property;
        SortIsDescending = isDescending;
        return this;
    }

    /// <summary>
    /// Applies all before options and sorting to the query (database-side)
    /// </summary>
    /// <param name="query">The queryable to modify</param>
    /// <returns>The modified queryable with all before options and sorting applied</returns>
    public IQueryable<TEntity> ApplyBefore(IQueryable<TEntity> query)
    {
        foreach (QueryOption<TEntity> option in BeforeOptions)
        {
            query = option.ApplyOption(query);
        }
        if (Sorting != null)
        {
            query = SortIsDescending
                ? query.OrderByDescending(Sorting)
                : query.OrderBy(Sorting);
        }
        return query;
    }

    /// <summary>
    /// Applies all after options to a collection (client-side navigation property loading)
    /// </summary>
    /// <param name="collection">The collection to process</param>
    /// <param name="context">The database context for loading navigation properties</param>
    /// <returns>The processed collection with all after options applied</returns>
    public async Task<IEnumerable<TEntity>> ApplyAfter(IEnumerable<TEntity> collection, DbContext context)
    {
        foreach (QueryOption<TEntity> option in AfterOptions)
        {
            collection = await option.ApplyOption(collection, context);
        }
        return collection;
    }

    /// <summary>
    /// Applies all after options to a single entity (client-side navigation property loading)
    /// </summary>
    /// <param name="entity">The entity to process</param>
    /// <param name="context">The database context for loading navigation properties</param>
    /// <returns>The entity with all after options applied</returns>
    public async Task<TEntity> ApplyAfter(TEntity entity, DbContext context)
    {
        foreach (QueryOption<TEntity> option in AfterOptions)
        {
            entity = await option.ApplyOption(entity, context);
        }
        return entity;
    }
}