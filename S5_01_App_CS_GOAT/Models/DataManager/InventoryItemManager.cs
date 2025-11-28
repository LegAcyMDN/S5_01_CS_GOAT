using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class InventoryItemManager : CrudRepository<InventoryItem>
    {
        public InventoryItemManager(CSGOATDbContext context) : base(context)
        {
        }

       
    }
}
