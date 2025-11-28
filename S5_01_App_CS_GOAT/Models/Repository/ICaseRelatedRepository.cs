using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ICaseRelatedRepository<TEntity>
{
    Task<TEntity?> GetByIdWithContentsAsync(int id);
}
