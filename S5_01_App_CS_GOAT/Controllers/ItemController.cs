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
        //IMapper mapper,
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


        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Item>>> GetAll()
        {
            IEnumerable<Item?> item = await manager.GetAllAsync();
            if (item == null || !item.Any())
                return NotFound();
            return Ok(item);
        }
    }
}


