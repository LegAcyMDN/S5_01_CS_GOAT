using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOAT.Controllers
{
    [ApiController]
    [Route("api/pricehistory")]
    public class PriceHistoryController : ControllerBase
    {
        private readonly PriceHistoryManager _manager;
        private readonly IConfiguration _configuration;

        public PriceHistoryController(IDataRepository<PriceHistory, int, string> manager, IConfiguration configuration)
        {
            _manager = (PriceHistoryManager)manager;
            _configuration = configuration;
        }

        [HttpGet("get/byinvitem")]
        public async Task<IActionResult> GetByInventoryItem(int inventoryItemId)
        {
            var authResult = JwtService.JwtAuth(_configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            var result = await _manager.GetByInventoryItemAsync(inventoryItemId, authResult);
            return Ok(result);
        }

        [HttpGet("get/aiprediction")]
        public async Task<IActionResult> GetAiPrediction(int inventoryItemId)
        {
            var authResult = JwtService.JwtAuth(_configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            var result = await _manager.GetAiPredictionAsync(inventoryItemId, authResult);
            return Ok(result);
        }
    }
}