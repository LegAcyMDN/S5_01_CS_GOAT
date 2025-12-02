namespace S5_01_Blazor_CS_GOAT.Service;

public interface IThreeDModelService<TEntity>
{
    Task<TEntity?> GetByIdAsync(int id);
}