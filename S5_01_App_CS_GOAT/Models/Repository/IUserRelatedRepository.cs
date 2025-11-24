namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IUserRelatedRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetByUserAsync(
        int userId,
        FilterOptions? filters = null,
        SortOptions? sorts = null);
}
