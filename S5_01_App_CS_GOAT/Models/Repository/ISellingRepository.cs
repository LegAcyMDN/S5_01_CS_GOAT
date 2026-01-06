using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface ISellingRepository
    {
        Task<int> SellAsync(int invItemId);

        Task<int> SellAsync(InventoryItem invItem);
    }
}
