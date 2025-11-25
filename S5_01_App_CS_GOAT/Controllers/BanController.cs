using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/bans")]
    [ApiController]
    public class BanController(
        IMapper mapper,
        IDataRepository<Ban, int, string> manager,
        CSGOATDbContext context,
        IConfiguration configuration) : ControllerBase
    {
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BanDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<Ban> bans = await authResult.GetByUser(manager, true);
            if (!bans.Any()) return NotFound();

            IEnumerable<BanDTO> userBansDTO = mapper.Map<IEnumerable<BanDTO>>(bans);
            return Ok(userBansDTO);
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<BanDTO>>> GetAll()
        {
            IEnumerable<Ban?> bans = await manager.GetAllAsync();

            if (bans == null || !bans.Any())
                return NotFound();

            // Mapping vers DTO
            IEnumerable<BanDTO> bansDTO = mapper.Map<IEnumerable<BanDTO>>(bans);
            return Ok(bansDTO);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(BanDTO banDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapping DTO vers entité
            Ban ban = mapper.Map<Ban>(banDTO);
            await manager.AddAsync(ban);

            BanDTO createdBanDTO = mapper.Map<BanDTO>(ban);
            return CreatedAtAction("GetAll", new { id = ban.UserId }, createdBanDTO);
        }

        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, BanDTO banDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Ban? banToUpdate = await manager.GetByIdAsync(id);
            if (banToUpdate == null)
            {
                return NotFound();
            }

            // Mapping DTO vers entité
            Ban ban = mapper.Map<Ban>(banDTO);
            await manager.UpdateAsync(banToUpdate, ban);

            return NoContent();
        }
    }
}