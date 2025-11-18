using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class InventoryItemManager(CSGOATDbContext context) : CrudRepository<InventoryItem>(context)
    {
        public override async Task<InventoryItem?> GetByKeyAsync(string name)
        {
            throw new NotImplementedException();
        }
    }
}
