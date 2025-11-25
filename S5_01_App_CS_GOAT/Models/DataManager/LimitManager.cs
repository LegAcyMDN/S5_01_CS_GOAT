using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class LimitManager(CSGOATDbContext context) : CrudRepository<Limit>(context)
    {
        public override async Task<Limit?> GetByKeyAsync(string str)
        {
            // Implémentation à définir selon vos besoins
            throw new NotImplementedException();
        }

        public async Task<Limit?> GetLimitTypeByNameAndDurationAsync(string? limitTypeName, string? durationName)
        {
            

            return await _context.LimitTypes.FirstOrDefaultAsync(lt => 
                lt.LimitTypeName == limitTypeName && 
                lt.DurationName == durationName);
        }
    }
}
