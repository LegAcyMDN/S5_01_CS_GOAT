using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class PriceHistoryManager: ReadRepository<PriceHistory, int>, IPriceHistoryRepository
    {
        protected readonly CSGOATDbContext _context;
        public PriceHistoryManager(CSGOATDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PriceHistory>?> PredictWithAI(Wear wear, int days = 30, bool limit = false)
        {
            HttpClient httpClient = new HttpClient();
            string flaskApiUrl = $"http://localhost:5555/api/price_history/predict_price/bywear/{wear.WearId}/{days}";
            HttpResponseMessage response = await httpClient.GetAsync(flaskApiUrl);
            if (!response.IsSuccessStatusCode) return null;
            await _context.Entry(wear).ReloadAsync();
            IEnumerable<PriceHistory> histories = wear.PriceHistories(true);
            if (limit) histories = histories.Where(p => p.PriceDate <= DateTime.Now.AddDays(days));
            return histories;
        }
    }
}
