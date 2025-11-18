using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class PaymentMethodManager(CSGOATDbContext context) : CrudRepository<PaymentMethod>(context)
    {
        public override async Task<PaymentMethod?> GetByKeyAsync(string name)
        {
            return await _context.Set<PaymentMethod>().FirstOrDefaultAsync(p => p.PaymentMethodName == name);
        }
    }
}
