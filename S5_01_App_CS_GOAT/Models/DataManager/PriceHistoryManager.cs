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
