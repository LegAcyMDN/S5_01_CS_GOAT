using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ISkinRelatedRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetByCase(
        int caseId,
        FilterOptions? filters = null,
        SortOptions? sorts = null);
}
