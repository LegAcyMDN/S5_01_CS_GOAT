using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/bans")]
    [ApiController]
    public class BanController(/*IMapper mapper,*/ IDataRepository<Ban, int, string> manager, CSGOATDbContext context) : ControllerBase
    {
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Ban? ban = await manager.GetByIdAsync(id);
            return ban == null ? NotFound() : Ok(ban);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<Ban>>> GetAll()
        {
            IEnumerable<Ban?> ban = await manager.GetAllAsync();
            if (ban == null || !ban.Any())
                return NotFound();
            return Ok(ban);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(Ban ban)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await manager.AddAsync(ban);

            return CreatedAtAction("Get", new { id = ban.UserId }, ban);
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, Ban ban)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Ban? banToUpdaten = await manager.GetByIdAsync(id);

            if (banToUpdaten == null)
            {
                return NotFound();
            }
            await manager.UpdateAsync(banToUpdaten, ban);

            return NoContent();
        }

    }
}
