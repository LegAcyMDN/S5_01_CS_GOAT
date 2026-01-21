using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages price history and AI price prediction for items
    /// </summary>
    [Route("api/PriceHistory")]
    [ApiController]
    public class PriceHistoryController(
        IReadableRepository<Wear, int> wearManager,
        IPriceHistoryRepository manager,
        IMapper mapper
    ) : ControllerBase
    {
        /// <summary>
        /// Get historical price data for a specific wear/item
        /// </summary>
        /// <param name="wearId">The ID of the wear</param>
        /// <returns>List of historical price points for the wear</returns>
        [HttpGet("bywear/{wearId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByWear(int wearId)
        {
            QueryOptions<Wear> options =
                new QueryOptions<Wear>()
                .After(w => w.WearClass.PriceHistories);
            Wear? wear = await wearManager.GetByIdAsync(wearId, options);
            if (wear == null)
            {
                return NotFound();
            }

            IEnumerable<PriceHistory> result = wear.PriceHistories(false);
            IEnumerable<PriceHistoryDTO> dto = mapper.Map<IEnumerable<PriceHistoryDTO>>(result);
            return Ok(dto);
        }


        /// <summary>
        /// Get AI-predicted future prices for an item using machine learning
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>Predicted price points based on AI analysis</returns>
        [HttpGet("aiprediction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetAiPrediction(int wearId)
        {
            QueryOptions<Wear> options =
                new QueryOptions<Wear>()
                .After(w => w.WearClass.PriceHistories);
            Wear? wear = await wearManager.GetByIdAsync(wearId, options);
            if (wear == null)
            {
                return NotFound();
            }

            IEnumerable<PriceHistory>? result = await manager.PredictWithAI(wear);
            if (result == null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            IEnumerable<PriceHistoryDTO> priceHistoryDTOs = mapper.Map<IEnumerable<PriceHistoryDTO>>(result);

            return Ok(priceHistoryDTOs);
        }
    }
}