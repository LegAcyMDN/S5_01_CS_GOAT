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
    [SetThreadPrincipal]
    public class FavoriteController(
        IMapper mapper,
        IDataRepository<Favorite, (int, int)> manager,
        IReadableRepository<Case, int> caseRepository,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Create a new favorite
        /// </summary>
        /// <param name="caseId">The case id to make a favorite for</param>
        /// <returns>The created Favorite object</returns>
        [HttpPost("create/{caseId}")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(int caseId)
        {
            Case? targetCase = await caseRepository.GetByIdAsyncNew(caseId);
            if (targetCase == null)
                return NotFound();

            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            Favorite? existing = await manager.GetByIdAsync((authResult.AuthUserId.Value, caseId));
            if (existing != null) return Conflict();

            Favorite favorite = new Favorite
            {
                CaseId = caseId,
                UserId = authResult.AuthUserId!.Value
            };

            await manager.AddAsync(favorite);
            return CreatedAtAction(null, new { id = favorite.UserId, favorite.CaseId });
        }

        /// <summary>
        /// Remove a favorite for the authenticated user
        /// </summary>
        /// <param name="caseId">The ID of the case</param>
        /// <returns>No content on success</returns>
        [HttpDelete("remove/{caseId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int caseId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            Favorite? favorite = await manager.GetByIdAsync((authResult.AuthUserId.Value, caseId));
            if (favorite == null) return NotFound();

            await manager.DeleteAsync(favorite);
            return NoContent();
        }
    }
}
