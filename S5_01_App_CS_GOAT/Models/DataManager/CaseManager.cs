using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class CaseManager(CSGOATDbContext context) : CrudRepository<Case>(context), ICaseRelatedRepository<Case>
    {
        public async Task<Case?> GetByIdWithContentsAsync(int id)
        {
            Case? caseResult = await _context.Set<Case>()
                .Include(c => c.CaseContents)
                .FirstOrDefaultAsync(c => c.CaseId == id);

            return caseResult;
        }
    }
}
