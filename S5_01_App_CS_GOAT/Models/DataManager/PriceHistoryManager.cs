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
    public class PriceHistoryManager: ReadRepository<PriceHistory, int>, IPriceHistoryRepository
    {
#if DEBUG
        private static string FLASK_URL = "http://localhost:5555";
#else
        private static string FLASK_URL = "https://iacsgoat-h6bkescydravhwf8.canadacentral-01.azurewebsites.net";
#endif

        protected readonly CSGOATDbContext _context;
        public PriceHistoryManager(CSGOATDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PriceHistory>?> PredictWithAI(Wear wear, int days = 30, bool limit = false)
        {
            HttpClient httpClient = new HttpClient();
            string flaskApiUrl = $"{FLASK_URL}/api/price_history/predict_price/bywear/{wear.WearId}/{days}";
            HttpResponseMessage response = await httpClient.GetAsync(flaskApiUrl);
            if (!response.IsSuccessStatusCode) return null;
            await _context.Entry(wear).ReloadAsync();
            IEnumerable<PriceHistory> histories = wear.PriceHistories(true);
            if (limit) histories = histories.Where(p => p.PriceDate <= DateTime.Now.AddDays(days));
            return histories;
        }
    }
}
