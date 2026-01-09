using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/PriceHistory")]
    [ApiController]
    public class PriceHistoryController(
        IReadableRepository<Wear, int> wearManager,
        IPriceHistoryRepository manager,
        IMapper mapper
    ) : ControllerBase
    {
        /// <summary>
        /// Get price history by wear
        /// </summary>
        /// <param name="wearId">The ID of the wear</param>
        /// <returns>Price history data for wear</returns>
        [HttpGet("bywear/{wearId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByWear(int wearId)
        {
            QueryOptions<Wear> options =
                new QueryOptions<Wear>()
                .After(w => w.WearClass.PriceHistories);
            Wear? wear = await wearManager.GetByIdAsyncNew(wearId, options);
            if (wear == null) return NotFound();
            IEnumerable<PriceHistory> result = wear.PriceHistories(false);
            IEnumerable<PriceHistoryDTO> dto = mapper.Map<IEnumerable<PriceHistoryDTO>>(result);
            return Ok(dto);
        }


        /// <summary>
        /// Get AI prediction for price history
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>AI prediction data</returns>
        [HttpGet("aiprediction")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetAiPrediction(int wearId)
        {
            QueryOptions<Wear> options =
                new QueryOptions<Wear>()
                .After(w => w.WearClass.PriceHistories);
            Wear? wear = await wearManager.GetByIdAsyncNew(wearId, options);
            if (wear == null) return NotFound();
            IEnumerable<PriceHistory>? result = await manager.PredictWithAI(wear);
            if (result == null) return StatusCode(StatusCodes.Status503ServiceUnavailable);
            IEnumerable<PriceHistoryDTO> priceHistoryDTOs = mapper.Map<IEnumerable<PriceHistoryDTO>>(result);

            return Ok(priceHistoryDTOs);
        }
    }
}