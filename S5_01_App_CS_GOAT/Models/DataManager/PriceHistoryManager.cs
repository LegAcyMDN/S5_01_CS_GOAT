using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class PriceHistoryManager : CrudRepository<PriceHistory>, IDataRepository<PriceHistory, int, string>
    {
        public PriceHistoryManager(CSGOATDbContext context) : base(context)
        {
        }

        

        public async Task<IEnumerable<PriceHistoryDTO>> GetByInventoryItemAsync(int wearId)
        {
            var priceHistories = await _context.PriceHistories
                .Where(ph => ph.WearId == wearId)
                .ToListAsync();

            return priceHistories.Select(ph => new PriceHistoryDTO
            {
                PriceDate = ph.PriceDate,
                PriceValue = ph.PriceValue,
                IsGuess = ph.GuessDate.HasValue
            });
        }
    }
}