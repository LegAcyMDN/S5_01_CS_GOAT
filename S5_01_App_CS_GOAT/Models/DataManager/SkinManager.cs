using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class SkinManager(CSGOATDbContext context) : CrudRepository<Skin>(context), ISkinRelatedRepository<Skin>
    {
        public async Task<IEnumerable<CaseContent>> GetByCase(int caseId, FilterOptions? filters = null, SortOptions? sorts = null)
        {
            IEnumerable<CaseContent> caseContents = await _context.Set<CaseContent>()
                .Include(cc => cc.Skin)
                    .ThenInclude(s => s.Item)
                        .ThenInclude(i => i.ItemType)
                .Include(cc => cc.Skin)
                    .ThenInclude(s => s.Rarity)
                .Include(cc => cc.Skin)
                    .ThenInclude(s => s.Wears)
                        .ThenInclude(w => w.PriceHistories)
                .Where(cc => cc.CaseId == caseId)
                .ToListAsync();
   
            return caseContents;
        }

    }
}
