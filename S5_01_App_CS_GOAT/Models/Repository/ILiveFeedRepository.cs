using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ILiveFeedRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetLiveFeedAsync(
        FilterOptions? filters = null,
        SortOptions? sorts = null);
}
