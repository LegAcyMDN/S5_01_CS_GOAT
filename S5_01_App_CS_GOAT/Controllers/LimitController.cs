using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/limit")]
    [ApiController]
    public class LimitController(
       IMapper mapper,
       IDataRepository<Limit, int, (int, int)> manager, IConfiguration configuration
       ) : ControllerBase
    {

        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LimitDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<Limit> limits = await authResult.GetByUser(manager, false);
            if (!limits.Any()) return NotFound();

            IEnumerable<LimitDTO> limitsDTO = mapper.Map<IEnumerable<LimitDTO>>(limits);
            return Ok(limitsDTO);
        }

        [HttpPatch("{userId}/{limitTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchLimit(int userId, int limitTypeId, [FromBody] LimitDTO limitDto)
        {
            if (limitDto == null)
                return BadRequest("LimitDTO cannot be null");

            Limit? existingLimit = await manager.GetByIdsAsync(userId, limitTypeId);
            
            if (existingLimit == null)
                return NotFound($"Limit not found for UserId: {userId} and LimitTypeId: {limitTypeId}");

            Dictionary<string, object> patchData = new Dictionary<string, object>
            {
                { nameof(Limit.LimitAmount), limitDto.LimitAmount }
            };
            
            await manager.PatchAsync(existingLimit, patchData);

            return NoContent();
        }
    }
}

