using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Manages historical price data retrieval and AI-based price predictions
    /// </summary>
    /// <remarks>
    /// Interfaces with external Flask service for machine learning-based price predictions.
    /// Supports both debug (localhost) and production (Azure) Flask endpoints.
    /// </remarks>
    public class PriceHistoryManager : ReadRepository<PriceHistory, int>, IPriceHistoryRepository
    {
        protected new readonly CSGOATDbContext _context;
        private readonly string _flaskUrl;

        public PriceHistoryManager(CSGOATDbContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _flaskUrl = configuration["Urls:FlaskService"] ?? "http://localhost:5555";
        }

        /// <summary>
        /// Retrieves historical price data and AI-predicted prices for an item wear
        /// </summary>
        /// <param name="wear">The item wear to retrieve price history for</param>
        /// <param name="days">The number of days to include in prediction (default: 30)</param>
        /// <param name="limit">Whether to limit results to within the specified days (default: false)</param>
        /// <returns>Collection of price history including predicted values, or null if prediction fails</returns>
        /// <remarks>
        /// This method calls the Flask service to generate AI-based price predictions.
        /// The predictions extend the historical price data with future price estimates.
        /// </remarks>
        public async Task<IEnumerable<PriceHistory>?> PredictWithAI(Wear wear, int days = 30, bool limit = false)
        {
            var httpClient = new HttpClient();
            string flaskApiUrl = $"{_flaskUrl}/api/price_history/predict_price/bywear/{wear.WearId}/{days}";
            HttpResponseMessage response = await httpClient.GetAsync(flaskApiUrl);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            await _context.Entry(wear).ReloadAsync();
            IEnumerable<PriceHistory> histories = wear.PriceHistories(true);
            if (limit)
            {
                histories = histories.Where(p => p.PriceDate <= DateTime.Now.AddDays(days));
            }

            return histories;
        }
    }
}
