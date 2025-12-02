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
        IDataRepository<InventoryItem, int> manager,
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
        /// Get detailed inventory item information by InventoryItemId
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>InventoryItemDetailDTO object</returns>
        [HttpGet("details/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            InventoryItem? item = await manager.GetByIdAsync(inventoryItemId);
            if (item == null || item.UserId != authResult.AuthUserId) return NotFound();

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
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Toggle favorite status of an inventory item by InventoryItemId
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>No content on success</returns>
        [HttpPatch("togglefavorite/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleFavorite(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            InventoryItem? inventory = await manager.GetByIdAsync(inventoryItemId);
            if (inventory == null) return NotFound();
            if (inventory.UserId != authResult.AuthUserId) return Forbid();

            inventory.IsFavorite = !inventory.IsFavorite;
            await manager.UpdateAsync(inventory, inventory);
            return NoContent();
        }

        /// <summary>
        /// Sell an inventory item by InventoryItemId (marks it as removed)
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>No content on success</returns>
        [HttpDelete("sell/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Sell(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            InventoryItem? inventory = await manager.GetByIdAsync(inventoryItemId);
            if (inventory == null || inventory.UserId != authResult.AuthUserId) return NotFound();

            throw new NotImplementedException();
        }
    }
}
