using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/UpgradeResult")]
    [ApiController]
    [SetThreadPrincipal]
    public class UpgradeResultController(
        IMapper mapper,
        IDataRepository<InventoryItem, int> invItemManager,
        IDataRepository<RandomTransaction, int> randTransManager,
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
        public async Task<IActionResult> GetByInventoryItem(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            QueryOptions<InventoryItem> options = new QueryOptions<InventoryItem>()
                .Before(i => i.UpgradeResults);
            InventoryItem? inventoryItem = await invItemManager.GetByIdAsyncNew(inventoryItemId, options);
            if (inventoryItem == null || inventoryItem.DependantUserId != authResult.AuthUserId) return NotFound();

            IEnumerable<UpgradeResultDTO> upgradeResultsDTO = mapper.Map<IEnumerable<UpgradeResultDTO>>(inventoryItem.UpgradeResults);
            return Ok(new GetOptions<UpgradeResultDTO>(Request, upgradeResultsDTO));
        }

        /// <summary>
        /// Get upgrade results by random transaction
        /// </summary>
        /// <param name="transactionId">The ID of the random transaction</param>
        /// <returns>Upgrade result data for random transaction</returns>
        [HttpGet("byrandomtransaction/{transactionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByRandomTransaction(int transactionId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            QueryOptions<RandomTransaction> options = new QueryOptions<RandomTransaction>()
                .Before(rt => rt.UpgradeResults);
            RandomTransaction? randomTransaction = await randTransManager.GetByIdAsyncNew(transactionId, options);
            if (randomTransaction == null || randomTransaction.DependantUserId != authResult.AuthUserId) return NotFound();

            IEnumerable<UpgradeResultDTO> upgradeResultsDTO = mapper.Map<IEnumerable<UpgradeResultDTO>>(randomTransaction.UpgradeResults);
            return Ok(new GetOptions<UpgradeResultDTO>(Request, upgradeResultsDTO));
        }
    }
}
