using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Case")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class CaseController(
        IMapper mapper,
        IReadableRepository<Case, int> manager,
        IDataRepository<Favorite, (int,int)> favoriteManager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get all cases
        /// </summary>
        /// <returns>List of all CaseDTO objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CaseDTO>>> GetAll()
        {
            IEnumerable<Case> caseResult = await manager.GetAllAsync();
            IEnumerable<CaseDTO> caseDTO = mapper.Map<IEnumerable<CaseDTO>>(caseResult);

            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated) return Ok(caseDTO);

            foreach (CaseDTO caseDto in caseDTO)
            {
                Favorite? favorite = await favoriteManager.GetByIdAsync(
                    (authResult.AuthUserId.Value,
                    caseDto.CaseId)
                );
                caseDto.IsFavorite = favorite != null;
            }

            return Ok(caseDTO);
        }

        /// <summary>
        /// Get case details by ID
        /// </summary>
        /// <param name="id">The ID of the case</param>
        /// <returns>CaseDetailDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            Case? result = await manager.GetByIdAsync(id);
            if (result == null) return NotFound();
            CaseDetailDTO caseDetailDTO = mapper.Map<CaseDetailDTO>(result);

            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated) return Ok(caseDetailDTO);

            Favorite? favorite = await favoriteManager.GetByIdAsync(
                (authResult.AuthUserId.Value,
                caseDetailDTO.CaseId)
            );
            caseDetailDTO.IsFavorite = favorite != null;
            return Ok(caseDetailDTO);
        }
    }
}
