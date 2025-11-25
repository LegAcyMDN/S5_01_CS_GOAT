using Microsoft.EntityFrameworkCore;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class PriceHistoryManager : CrudRepository<PriceHistory>, IDataRepository<PriceHistory, int, string>
    {
        public PriceHistoryManager(CSGOATDbContext context) : base(context)
        {
        }

        public override async Task<PriceHistory?> GetByKeyAsync(string key)
        {
            return await _context.PriceHistories.FirstOrDefaultAsync(ph => ph.PriceHistoryId.ToString() == key);
        }

        public async Task<IEnumerable<PriceHistoryDTO>> GetByInventoryItemAsync(int inventoryItemId, AuthResult authResult)
        {
            if (!authResult.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var priceHistories = await _context.PriceHistories
                .Where(ph => ph.WearId == inventoryItemId)
                .ToListAsync();

            return priceHistories.Select(ph => new PriceHistoryDTO
            {
                PriceDate = ph.PriceDate,
                PriceValue = ph.PriceValue,
                IsGuess = ph.GuessDate.HasValue
            });
        }

        public async Task<IEnumerable<PriceHistoryDTO>> GetAiPredictionAsync(int inventoryItemId, AuthResult authResult)
        {
            if (!authResult.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var priceHistories = await _context.PriceHistories
                .Where(ph => ph.WearId == inventoryItemId && ph.GuessDate.HasValue)
                .ToListAsync();

            return priceHistories.Select(ph => new PriceHistoryDTO
            {
                PriceDate = ph.PriceDate,
                PriceValue = ph.PriceValue,
                IsGuess = true
            });
        }
    }
}