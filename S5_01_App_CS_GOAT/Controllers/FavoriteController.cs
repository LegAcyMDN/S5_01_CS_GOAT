using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Favorite")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class FavoriteController(
        IMapper mapper,
        IDataRepository<Favorite, int, string> manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Create a new favorite
        /// </summary>
        /// <param name="favorite">The Favorite object to create</param>
        /// <returns>The created Favorite object</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] Favorite favorite)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            if (favorite.UserId != authResult.AuthUserId)
                return Forbid();

            await manager.AddAsync(favorite);
            return CreatedAtAction(null, new { id = favorite.UserId, favorite.CaseId }, favorite);
        }

        /// <summary>
        /// Remove a favorite by ID
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="caseId">The ID of the case</param>
        /// <returns>No content on success</returns>
        [HttpDelete("remove/{userId}/{caseId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int userId, int caseId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (userId != authResult.AuthUserId)
                return Forbid();

            Favorite? favorite = await manager.GetByIdsAsync(userId, caseId);
            if (favorite == null) return NotFound();

            await manager.DeleteAsync(favorite);
            return NoContent();
        }
    }
}
