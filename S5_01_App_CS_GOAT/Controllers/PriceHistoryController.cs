using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/PriceHistory")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class PriceHistoryController(
        IReadableRepository<Wear, int> wearManager,
       IReadableRepository<PriceHistory,int> manager,
        IMapper mapper
    ) : ControllerBase
    {
        /// <summary>
        /// Get price history by inventory item
        /// </summary>
        /// <param name="wearId">The ID of the wear</param>
        /// <returns>Price history data for wear</returns>
        [HttpGet("bywear/{wearId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByInventoryItem(int wearId)
        {
            Wear? wear = await wearManager.GetByIdAsync(wearId, "WearType.PriceHistories");
            if (wear == null) return NotFound();
            IEnumerable<PriceHistory> result = wear.PriceHistories();
            IEnumerable<PriceHistoryDTO> dto = mapper.Map<IEnumerable<PriceHistoryDTO>>(result);
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