using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface IPriceHistoryRepository : IReadableRepository<PriceHistory, int>
    {
        public Task<IEnumerable<PriceHistory>?> PredictWithAI(InventoryItem invItem, int days = 30, bool limit = false)
        {
            return PredictWithAI(invItem.Wear, days, limit);
        }

        public Task<IEnumerable<PriceHistory>?> PredictWithAI(Wear wear, int days = 30, bool limit = false);
    }
}
