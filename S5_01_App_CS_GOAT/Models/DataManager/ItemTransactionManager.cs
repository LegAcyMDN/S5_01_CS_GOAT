using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class ItemTransactionManager(CSGOATDbContext context) : CrudRepository<ItemTransaction>(context)
    {
        public override async Task<ItemTransaction?> GetByKeyAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
