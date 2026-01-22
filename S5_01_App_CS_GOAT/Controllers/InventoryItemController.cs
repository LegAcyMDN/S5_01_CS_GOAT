using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages user inventory items including viewing, upgrading, selling, and favorites
    /// </summary>
    [Route("api/InventoryItem")]
    [ApiController]
    [SetThreadPrincipal]
    public class InventoryItemController(
        IDataRepository<InventoryItem, int> manager,
        IUpgradeRepository upgradeService,
        ISellingRepository sellingService,
        IMapper mapper,
        IConfiguration configuration
        ) : ControllerBase
    {
        /// <summary>
        /// Get inventory items for the authenticated user (active items only)
        /// </summary>
        /// <returns>List of InventoryItemDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            QueryOptions<InventoryItem> queryOptions = new QueryOptions<InventoryItem>()
                .Before(i => i.RemovedOn == null)
                .Before(i => i.Wear.Skin.Rarity);
            IEnumerable<InventoryItem> inventoryItems = await authResult.GetByUser(
                manager, false, queryOptions);
            IEnumerable<InventoryItemDTO> inventory = mapper.Map<IEnumerable<InventoryItemDTO>>(inventoryItems);
            return Ok(new GetOptions<InventoryItemDTO>(Request, inventory));
        }

        /// <summary>
        /// Get detailed inventory item information by InventoryItemId
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>InventoryItemDetailDTO object with full details</returns>
        [HttpGet("details/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            QueryOptions<InventoryItem> options =
                new QueryOptions<InventoryItem>()
                .Before(i => i.Wear.Skin.Rarity,
                    i => i.Wear.Skin.Item.ItemType,
                    i => i.Wear.WearType)
                .After(i => i.Wear.WearClass.PriceHistories);
            InventoryItem? item = await manager.GetByIdAsync(inventoryItemId, options);
            if (item == null || item.UserId != authResult.AuthUserId)
            {
                return NotFound();
            }

            InventoryItemDetailDTO? inventory = mapper.Map<InventoryItemDetailDTO>(item);
            return Ok(inventory);
        }

        /// <summary>
        /// Upgrade inventory items with provably fair randomization
        /// </summary>
        /// <param name="dto">Upgrade parameters including target skin, items, and monetary value</param>
        /// <returns>UpgradeOutputDTO with probabilities and results</returns>
        [HttpPost("upgrade/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Upgrade([FromBody] UpgradeInputDTO dto)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            /*try
            {*/
            UpgradeOutputDTO output = await upgradeService.UpgradeAsync(dto, authResult.AuthUserId!.Value);
            return Ok(output);
            /*}
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }*/
        }

        /// <summary>
        /// Toggle favorite status of an inventory item
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>No content on success</returns>
        [HttpPatch("togglefavorite/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleFavorite(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            InventoryItem? inventory = await manager.GetByIdAsync(inventoryItemId);
            if (inventory == null)
            {
                return NotFound();
            }

            if (inventory.UserId != authResult.AuthUserId)
            {
                return Forbid();
            }

            inventory.IsFavorite = !inventory.IsFavorite;
            await manager.UpdateAsync(inventory, inventory);
            return NoContent();
        }

        /// <summary>
        /// Sell an inventory item and credit wallet (marks item as removed)
        /// </summary>
        /// <param name="inventoryItemId">The ID of the inventory item</param>
        /// <returns>No content (204) on success, other status codes based on sell service result</returns>
        [HttpDelete("sell/{inventoryItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status410Gone)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Sell(int inventoryItemId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            InventoryItem? inventory = await manager.GetByIdAsync(inventoryItemId);
            if (inventory == null || inventory.UserId != authResult.AuthUserId)
            {
                return NotFound();
            }

            int responseCode = await sellingService.SellAsync(inventoryItemId);
            return StatusCode(responseCode);
        }
    }
}
