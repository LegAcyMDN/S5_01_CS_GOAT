using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/PriceHistory")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class PriceHistoryController(
        IDataRepository<PriceHistory, int, string> manager
    ) : ControllerBase
    {
        private readonly PriceHistoryManager _manager = (PriceHistoryManager)manager;

        /// <summary>
        /// Get price history by inventory item
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>Price history data for the inventory item</returns>
        [HttpGet("byinvitem/{wearId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByInventoryItem(int wearId)
        {
            IEnumerable<PriceHistoryDTO> result = await _manager.GetByInventoryItemAsync(wearId);
            return Ok(result);
        }

        /// <summary>
        /// Get AI prediction for price history
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>AI prediction data</returns>
        [HttpGet("aiprediction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status501NotImplemented)]
        public async Task<IActionResult> GetAiPrediction(int wearId)
        {
            throw new NotImplementedException();
        }
    }
}