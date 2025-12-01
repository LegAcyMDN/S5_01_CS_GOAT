namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ITypeRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetTypeByNameAsync(string typeName);
}