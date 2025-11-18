using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using System;

namespace S5_01_App_CS_GOAT.Controllers
{
   
    [Route("api/items")]
    [ApiController]
    public class ItemController(
        IMapper mapper,
        IDataRepository<Item, int, string> manager,
        CSGOATDbContext context
        ) : ControllerBase
    {
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Item? result = await manager.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("remove/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            ActionResult<Item?> item = await manager.GetByIdAsync(id);
            if (item.Value == null)
                return NotFound();
            await manager.DeleteAsync(item.Value);
            return NoContent();
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Item>>> GetAll()
        {
            IEnumerable<Item?> item = await manager.GetAllAsync();
            if (item == null || !item.Any())
                return NotFound();
            return Ok(item);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create( Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await manager.AddAsync(item);

            return CreatedAtAction("Get", new { id = item.ItemId }, item);
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id,  Item item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Item? itemToUpdate = await manager.GetByIdAsync(id);

            if (itemToUpdate == null)
            {
                return NotFound();
            }
            await manager.UpdateAsync(itemToUpdate, item);

            return NoContent();
        }
    }
}


