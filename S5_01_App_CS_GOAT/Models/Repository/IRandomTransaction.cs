namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface IRandomTransactiony<TEntity> : IDataRepository<TEntity,int,string>
    {
        Task<IEnumerable<TEntity>> GetRandomTransactionsAsync(int count);
    }
}
