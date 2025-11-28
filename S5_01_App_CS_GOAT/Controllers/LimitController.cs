using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Limit")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class LimitController(
       IMapper mapper,
       IDataRepository<Limit, (int,int)> manager, 
       IConfiguration configuration
       ) : ControllerBase
    {

        /// <summary>
        /// Get limits for the authenticated user
        /// </summary>
        /// <returns>List of LimitDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LimitDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<Limit> limits = await authResult.GetByUser(manager, false);

            IEnumerable<LimitDTO> limitsDTO = mapper.Map<IEnumerable<LimitDTO>>(limits);
            return Ok(limitsDTO);
        }

        /// <summary>
        /// Update a limit for a user
        /// </summary>
        /// <param name="userId">The user ID</param>
        /// <param name="limitTypeId">The limit type ID</param>
        /// <param name="limitDto">The updated limit data</param>
        /// <returns>No content on success</returns>
        [HttpPatch("update/{userId}/{limitTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int userId, int limitTypeId, [FromBody] LimitDTO limitDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            Limit? existingLimit = await manager.GetByIdAsync((userId, limitTypeId));
            
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

