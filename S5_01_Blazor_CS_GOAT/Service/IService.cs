namespace S5_01_Blazor_CS_GOAT.Service;

public interface IService<TEntity>
{
    Task<List<TEntity>?> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity updatedEntity);
    Task DeleteAsync(int id);
    Task<TEntity?> GetByNameAsync(string name);
    
    Task<List<TEntity>?> GetByCaseIdAsync(int id);
}