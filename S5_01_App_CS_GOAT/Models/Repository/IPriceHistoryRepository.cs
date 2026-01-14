using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Manages historical price data and provides AI-powered price predictions
    /// </summary>
    public interface IPriceHistoryRepository : IReadableRepository<PriceHistory, int>
    {
        /// <summary>
        /// Predicts future price history for an inventory item using AI
        /// </summary>
        /// <param name="invItem">The inventory item to predict prices for</param>
        /// <param name="days">Number of days ahead to predict (default 30)</param>
        /// <param name="limit">If true, limits predictions to the specified number of days</param>
        /// <returns>Collection of predicted PriceHistory records; null if prediction fails</returns>
        public Task<IEnumerable<PriceHistory>?> PredictWithAI(InventoryItem invItem, int days = 30, bool limit = false)
        {
            return PredictWithAI(invItem.Wear, days, limit);
        }

        /// <summary>
        /// Predicts future price history for a wear/skin using AI
        /// </summary>
        /// <param name="wear">The wear/skin to predict prices for</param>
        /// <param name="days">Number of days ahead to predict (default 30)</param>
        /// <param name="limit">If true, limits predictions to the specified number of days</param>
        /// <returns>Collection of predicted PriceHistory records; null if prediction fails</returns>
        public Task<IEnumerable<PriceHistory>?> PredictWithAI(Wear wear, int days = 30, bool limit = false);
    }
}
