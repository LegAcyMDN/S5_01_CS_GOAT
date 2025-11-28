using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using System.Linq.Expressions;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class RandomTransactionManager(CSGOATDbContext context) : CrudRepository<RandomTransaction>(context), IRandomTransactiony<RandomTransaction>
    {
        

        public async Task<IEnumerable<RandomTransaction>> GetRandomTransactionsAsync(int count)
        {
            IEnumerable<RandomTransaction> transactions = await context.RandomTransactions
                .Include(t => t.User)
                .Where(t => t.CaseId != null)
                .OrderBy(t => EF.Functions.Random())
                .Take(count)
                .ToListAsync();

            foreach (RandomTransaction? transaction in transactions)
            {
                transaction.User = null;
            }

            return transactions;
        }
    }
}
