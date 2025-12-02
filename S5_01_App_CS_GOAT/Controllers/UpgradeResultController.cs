using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/UpgradeResult")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class UpgradeResultController(
        IMapper mapper,
        IDataRepository<UpgradeResult, int> manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get upgrade results by inventory item
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>Upgrade result data for inventory item</returns>
        [HttpGet("byinventoryitem/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UpgradeResultDTO>>> GetByInventoryItem(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<UpgradeResult> upgradeResults = await manager.GetAllAsync(ur => ur.InventoryItemId == inventoryItemId);
            IEnumerable<UpgradeResultDTO> upgradeResultsDTO = mapper.Map<IEnumerable<UpgradeResultDTO>>(upgradeResults);
            return Ok(upgradeResultsDTO);
        }

        /// <summary>
        /// Get upgrade results by random transaction
        /// </summary>
        /// <param name="transactionId">The ID of the random transaction</param>
        /// <returns>Upgrade result data for random transaction</returns>
        [HttpGet("byrandomtransaction/{transactionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<UpgradeResultDTO>>> GetByRandomTransaction(int transactionId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<UpgradeResult> upgradeResults = await manager.GetAllAsync(ur => ur.InventoryItemId == transactionId);
            IEnumerable<UpgradeResultDTO> upgradeResultsDTO = mapper.Map<IEnumerable<UpgradeResultDTO>>(upgradeResults);
            return Ok(upgradeResultsDTO);
        }
    }
}
