using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface IWearRelatedRepository<TEntity>
    {
        Task<IEnumerable<TEntity>> GetBy3dModelAsync(
       int modelID,
       FilterOptions? filters = null,
       SortOptions? sorts = null);
    }
}
