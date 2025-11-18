using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using System;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class CaseManager(CSGOATDbContext context) : CrudRepository<Case>(context)
    {
        public override async Task<Case?> GetByKeyAsync(string name)
        {
            return await _context.Set<Case>().FirstOrDefaultAsync(c => c.CaseName == name);
        }
    }
}
