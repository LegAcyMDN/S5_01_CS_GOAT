using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/InventoryItem")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class InventoryItemController(
        IDataRepository<InventoryItem, int, string> manager,
        IToggleRepository<InventoryItem> toggleManager,
        IMapper mapper,
        IConfiguration configuration
        ) : ControllerBase
    {
        /// <summary>
        /// Get inventory items for the authenticated user
        /// </summary>
        /// <returns>List of InventoryItemDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<InventoryItem> inventoryItems = await authResult.GetByUser(manager, false);

            IEnumerable<InventoryItemDTO> inventory = inventoryItems
                .Select(item => mapper.Map<InventoryItemDTO>(item));
            
            return Ok(inventory);
        }

        /// <summary>
        /// Get detailed inventory item information for the authenticated user
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>InventoryItemDetailDTO object</returns>
        [HttpGet("details/{wearId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int wearId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            InventoryItem? item = await manager.GetByIdsAsync(authResult.AuthUserId, wearId);
            if (item == null) return NotFound();

            InventoryItemDetailDTO? inventory = mapper.Map<InventoryItemDetailDTO>(item);
            return Ok(inventory);
        }

        /// <summary>
        /// Upgrade an inventory item
        /// </summary>
        [HttpPost("upgrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upgrade()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Toggle favorite status of an inventory item
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>No content on success</returns>
        [HttpPatch("togglefavorite/{wearId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleFavorite(int wearId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            InventoryItem? inventory = await manager.GetByIdsAsync(authResult.AuthUserId.Value, wearId);
            if (inventory == null) return NotFound();

            await toggleManager.ToggleByIdsAsync(authResult.AuthUserId.Value, wearId);
            return NoContent();
        }

        /// <summary>
        /// Sell an inventory item (marks it as removed)
        /// </summary>
        /// <param name="wearId">The ID of the wear/item</param>
        /// <returns>No content on success</returns>
        [HttpDelete("sell/{wearId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Sell(int wearId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            InventoryItem? inventory = await manager.GetByIdsAsync(authResult.AuthUserId.Value, wearId);
            if (inventory == null) return NotFound();

            inventory.RemovedOn = DateTime.Now;
            await manager.UpdateAsync(inventory, inventory);
            return NoContent();
        }
    }
}
