namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IToggleRepository<TEntity>
{
    Task ToggleByIdsAsync(int id1, int id2);
}
