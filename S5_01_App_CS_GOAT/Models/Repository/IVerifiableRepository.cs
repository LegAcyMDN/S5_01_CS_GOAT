namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IVerifiableRepository<TEntity>
{
    Task<bool> VerifyAsync(int id, string code);
}
