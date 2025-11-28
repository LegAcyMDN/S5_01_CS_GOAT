using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class TransactionManager : CrudRepository<Transaction>
    {
        public TransactionManager(CSGOATDbContext context) : base(context)
        {
        }

      
        public async Task SetCancelledOnAsync(int id)
        {
            Transaction? transaction = await GetByIdAsync(id);
            if (transaction != null)
            {
                transaction.CancelledOn = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
