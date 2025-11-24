using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class MoneyTransactionManager(CSGOATDbContext context) : CrudRepository<MoneyTransaction>(context)
    {
        public override async Task<MoneyTransaction?> GetByKeyAsync(string name)
        {
            return await _context.Set<MoneyTransaction>().FirstOrDefaultAsync(p => p.PaymentMethod.PaymentMethodName == name);
        }
    }
}
