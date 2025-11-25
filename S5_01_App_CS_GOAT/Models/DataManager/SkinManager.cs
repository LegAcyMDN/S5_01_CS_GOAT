using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class SkinManager(CSGOATDbContext context) : CrudRepository<Skin>(context), ISkinRelatedRepository<Skin>
    {
        public async Task<IEnumerable<Skin>> getByCase(int caseId, FilterOptions? filters = null, SortOptions? sorts = null)
        {
            IEnumerable<CaseContent> caseContents = await _context.Set<CaseContent>().Where(cc => cc.CaseId == caseId).ToListAsync();
   
            return caseContents.Select(cc => cc.Skin);
        }

        public override async Task<Skin?> GetByKeyAsync(string name)
        {
            return await _context.Set<Skin>().FirstOrDefaultAsync(s => s.SkinName == name);
        }
    }
}
