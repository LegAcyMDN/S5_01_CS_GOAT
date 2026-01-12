using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace S5_01_App_CS_GOAT.Services;

public class QueryOption<TEntity> where TEntity : class
{
    private readonly Expression<Func<TEntity, bool>>? Where;
    private readonly Expression<Func<TEntity, object?>>? Include;
    private readonly string? Path;

    public QueryOption(Expression<Func<TEntity, bool>>? where, Expression<Func<TEntity, object?>>? include, string? includePath)
    {
        Where = where;
        Include = include;
        Path = includePath;
    }

    public QueryOption(Expression<Func<TEntity, bool>> where)
        : this(where, null, null) { }

    public QueryOption(Expression<Func<TEntity, object?>> include)
        : this(null, include, null) { }

    public QueryOption(string include)
        : this(null, null, include) { }

    public IQueryable<TEntity> ApplyOption(IQueryable<TEntity> query)
    {
        if (Include != null)
        {
            query = query.Include(Include);
        }
        if (Path != null)
        {
            query = query.Include(Path);
        }
        if (Where != null)
        {
            query = query.Where(Where);
        }
        return query;
    }

    public async Task<IEnumerable<TEntity>> ApplyOption(IEnumerable<TEntity> collection, DbContext context)
    {
        if (Include != null && collection.Any())
        {
            List<string> path = GetNavigation(Include);
            await LoadNavigation(collection, context, path);
        }
        if (Path != null && collection.Any())
        {
            string[] segments = Path.Split('.');
            List<string> path = segments.ToList();
            await LoadNavigation(collection, context, path);
        }
        if (Where != null)
        {
            collection = collection.AsQueryable().Where(Where);
        }
        return collection;
    }

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

    private static List<string> GetNavigation(Expression<Func<TEntity, object?>> include)
    {
        List<string> path = new();
        Expression current = include.Body;

        // Convert unary ("x.y") expressions to member expressions
        if (current is UnaryExpression unaryExpr &&
            unaryExpr.NodeType == ExpressionType.Convert)
        {
            current = unaryExpr.Operand;
        }

        while (current is MemberExpression memberExpr)
        {
            path.Insert(0, memberExpr.Member.Name);
            current = memberExpr.Expression;
        }

        if (path.Count == 0)
            throw new ArgumentException($"{include} is invalid. Must be a member access expression.", nameof(include));

        return path;
    }

    private async Task LoadNavigation<T>(IEnumerable<T> collection, DbContext context, List<string> path)
    {
        foreach (T entity in collection)
        {
            await LoadNavigation(entity, context, path);
        }
    }

    private async Task LoadNavigation<T>(T? entity, DbContext context, List<string> path)
    {
        if (entity == null) return;
        string propertyName = path[0];
        List<string> remainingPath = path.Skip(1).ToList();

        EntityEntry entry = context.Entry(entity);
        NavigationEntry navigation = entry.Navigation(propertyName);
        if (navigation.IsLoaded == false)
            await navigation.LoadAsync();

        if (remainingPath.Count == 0) return;

        if (navigation.Metadata.IsCollection)
        {
            IEnumerable<object?>? relatedEntities = navigation.CurrentValue as IEnumerable<object?>;
            if (relatedEntities == null) return;
            foreach (object? relatedEntity in relatedEntities)
            {
                if (relatedEntity == null) continue;
                await LoadNavigation(relatedEntity, context, remainingPath);
            }
        }
        else
        {
            object? relatedEntity = navigation.CurrentValue;
            if (relatedEntity == null) return;
            await LoadNavigation(relatedEntity, context, remainingPath);
        }
    }
}

public class QueryOptions<TEntity> where TEntity : class
{
    private List<QueryOption<TEntity>> BeforeOptions = new();
    private List<QueryOption<TEntity>> AfterOptions = new();
    private Expression<Func<TEntity, object?>>? Sorting;
    private bool SortIsDescending = false;

    public QueryOptions() { }

    public QueryOptions<TEntity> Before(params QueryOption<TEntity>[] options)
    {
        BeforeOptions.AddRange(options);
        return this;
    }

    public QueryOptions<TEntity> Before(params Expression<Func<TEntity, bool>>[] wheres)
    {
        foreach (var where in wheres)
        {
            BeforeOptions.Add(new QueryOption<TEntity>(where));
        }
        return this;
    }

    public QueryOptions<TEntity> Before(params Expression<Func<TEntity, object?>>[] includes)
    {
        foreach (var include in includes)
        {
            BeforeOptions.Add(new QueryOption<TEntity>(include));
        }
        return this;
    }

    public QueryOptions<TEntity> Before(params string[] paths)
    {
        foreach (string path in paths)
        {
            BeforeOptions.Add(new QueryOption<TEntity>(path));
        }
        return this;
    }

    public QueryOptions<TEntity> After(params QueryOption<TEntity>[] options)
    {
        AfterOptions.AddRange(options);
        return this;
    }

    public QueryOptions<TEntity> After(params Expression<Func<TEntity, bool>>[] wheres)
    {
        foreach (var where in wheres)
        {
            AfterOptions.Add(new QueryOption<TEntity>(where));
        }
        return this;
    }

    public QueryOptions<TEntity> After(params Expression<Func<TEntity, object?>>[] includes)
    {
        foreach (var include in includes)
        {
            AfterOptions.Add(new QueryOption<TEntity>(include));
        }
        return this;
    }

    public QueryOptions<TEntity> After(params string[] paths)
    {
        foreach (string path in paths)
        {
            AfterOptions.Add(new QueryOption<TEntity>(path));
        }
        return this;
    }

    public QueryOptions<TEntity> OrderBy(
        Expression<Func<TEntity, object?>> property,
        bool isDescending = false)
    {
        Sorting = property;
        SortIsDescending = isDescending;
        return this;
    }

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

    public async Task<IEnumerable<TEntity>> ApplyAfter(IEnumerable<TEntity> collection, DbContext context)
    {
        foreach (QueryOption<TEntity> option in AfterOptions)
        {
            collection = await option.ApplyOption(collection, context);
        }
        return collection;
    }

    public async Task<TEntity> ApplyAfter(TEntity entity, DbContext context)
    {
        foreach (QueryOption<TEntity> option in AfterOptions)
        {
            entity = await option.ApplyOption(entity, context);
        }
        return entity;
    }
}