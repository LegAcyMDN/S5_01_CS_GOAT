namespace S5_01_App_CS_GOAT.Controllers
{
    using AutoMapper;
    using global::S5_01_App_CS_GOAT.Models.EntityFramework;
    using global::S5_01_App_CS_GOAT.Models.Repository;
    using Microsoft.AspNetCore.Mvc;

    namespace S5_01_App_CS_GOAT.Controllers
    {

        [Route("api/inventorys")]
        [ApiController]
        public class InventoryItemController(
            // IMapper mapper,
            IDataRepository<InventoryItem, int, string> manager,
            CSGOATDbContext context
            ) : ControllerBase
        {
            [HttpGet("details/{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Get(int id)
            {
                InventoryItem? inventory = await manager.GetByIdAsync(id);
                return inventory == null ? NotFound() : Ok(inventory);
            }

            [HttpDelete("remove/{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Delete(int id)
            {
                InventoryItem? inventory = await manager.GetByIdAsync(id);
                if (inventory == null)
                    return NotFound();
                await manager.DeleteAsync(inventory);
                return NoContent();
            }

            [HttpGet("all")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<ActionResult<IEnumerable<InventoryItem>>> GetAll()
            {
                IEnumerable<InventoryItem?> inventory = await manager.GetAllAsync();
                if (inventory == null || !inventory.Any())
                    return NotFound();
                return Ok(inventory);
            }

            [HttpPost("create")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Create(InventoryItem inventory)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await manager.AddAsync(inventory);

                return CreatedAtAction("Get", new { id = inventory.WearId, inventory.UserId }, inventory);
            }

            [HttpPut("update/{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Update(int id, InventoryItem inventory)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                InventoryItem? inventoryToUpdate = await manager.GetByIdAsync(id);

                if (inventoryToUpdate == null)
                {
                    return NotFound();
                }
                await manager.UpdateAsync(inventoryToUpdate, inventory);

                return NoContent();
            }
        }
    }



}
