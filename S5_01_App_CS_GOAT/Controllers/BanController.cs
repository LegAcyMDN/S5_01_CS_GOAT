using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Ban")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class BanController(
        IMapper mapper,
        IDataRepository<Ban, int, string> manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get all bans (admin only)
        /// </summary>
        /// <returns>List of all BanDTO objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BanDTO>>> GetAll()
        {
            IEnumerable<Ban> bans = await manager.GetAllAsync();

            IEnumerable<BanDTO> bansDTO = mapper.Map<IEnumerable<BanDTO>>(bans);
            return Ok(bansDTO);
        }

        /// <summary>
        /// Get bans for the authenticated user
        /// </summary>
        /// <returns>List of BanDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BanDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<Ban> bans = await authResult.GetByUser(manager, true);

            IEnumerable<BanDTO> userBansDTO = mapper.Map<IEnumerable<BanDTO>>(bans);
            return Ok(userBansDTO);
        }

        /// <summary>
        /// Create a new ban (admin only)
        /// </summary>
        /// <param name="banDTO">The BanDTO object to create</param>
        /// <returns>The created BanDTO object</returns>
        [HttpPost("create")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(BanDTO banDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Ban ban = mapper.Map<Ban>(banDTO);
            await manager.AddAsync(ban);

            BanDTO createdBanDTO = mapper.Map<BanDTO>(ban);
            return CreatedAtAction("GetAll", new { id = ban.UserId }, createdBanDTO);
        }

        /// <summary>
        /// Update an existing ban (admin only)
        /// </summary>
        /// <param name="id">The ID of the ban to update</param>
        /// <param name="banDTO">The updated BanDTO object</param>
        /// <returns>No content on success</returns>
        [HttpPut("update/{id}")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, BanDTO banDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Ban? banToUpdate = await manager.GetByIdAsync(id);
            if (banToUpdate == null) return NotFound();

            Ban ban = mapper.Map<Ban>(banDTO);
            await manager.UpdateAsync(banToUpdate, ban);
            return NoContent();
        }
    }
}