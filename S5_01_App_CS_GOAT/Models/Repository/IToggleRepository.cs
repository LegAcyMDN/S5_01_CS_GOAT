namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IToggleRepository<TEntity>
{
    Task ToggleAsync(int id);
}
