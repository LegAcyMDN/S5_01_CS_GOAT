using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Manages the selling of inventory items and wallet transactions
    /// </summary>
    public interface ISellingRepository
    {
        /// <summary>
        /// Sells an inventory item by ID and adds the current price to user's wallet
        /// </summary>
        /// <param name="invItemId">The ID of the inventory item to sell</param>
        /// <returns>HTTP status code indicating result (204 for success, 404 for not found, 410 for already removed, 503 for price unavailable)</returns>
        Task<int> SellAsync(int invItemId);

        /// <summary>
        /// Sells an inventory item and adds the current price to user's wallet
        /// </summary>
        /// <param name="invItem">The inventory item to sell</param>
        /// <returns>HTTP status code indicating result (204 for success, 410 for already removed, 503 for price unavailable)</returns>
        Task<int> SellAsync(InventoryItem invItem);
    }
}
