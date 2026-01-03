namespace S5_01_App_CS_GOAT.Models.Repository;

public interface ITypeRepository<TEntity>:
    IReadableRepository<TEntity, int>
    where TEntity : class
{
    Task<TEntity?> GetTypeByNameAsync(string typeName);
}