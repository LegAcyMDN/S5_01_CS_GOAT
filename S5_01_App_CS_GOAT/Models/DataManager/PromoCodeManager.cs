using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Services;
using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class PromoCodeManager : CrudRepository<PromoCode>
    {
        public PromoCodeManager(CSGOATDbContext context) : base(context) { }

        public override async Task<PromoCode?> GetByKeyAsync(string code)
        {
            return await _context.PromoCodes.FirstOrDefaultAsync(p => p.Code == code);
        }
    }
}