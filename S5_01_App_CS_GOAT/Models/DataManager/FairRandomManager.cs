using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class FairRandomManager(CSGOATDbContext context) : CrudRepository<FairRandom>(context)
    {
        public override async Task<FairRandom?> GetByKeyAsync(string combinedHash)
        {
            return await _context.Set<FairRandom>().FirstOrDefaultAsync(f => f.CombinedHash == combinedHash);
        }

        public async Task<IEnumerable<FairRandom>> GetByUserAsync(int userId, FilterOptions? filters = null, SortOptions? sorts = null)
        {
            return await _context.Set<FairRandom>().Where(f => f.RandomTransaction != null && f.RandomTransaction.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<FairRandom>> GetAllAsync(FilterOptions? filters = null, SortOptions? sorts = null)
        {
            IQueryable<FairRandom> query = _context.Set<FairRandom>();

            if (filters?.Filters != null)
            {
                foreach (var filter in filters.Filters)
                {
                    // Apply filtering logic here
                }
            }

            if (sorts?.Sorts != null)
            {
                foreach (var sort in sorts.Sorts)
                {
                    // Apply sorting logic here
                }
            }

            return await query.ToListAsync();
        }
    }
}
