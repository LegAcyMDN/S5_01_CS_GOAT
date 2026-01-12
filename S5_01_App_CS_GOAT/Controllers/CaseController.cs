using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Case")]
    [ApiController]
    [SetThreadPrincipal]
    public class CaseController(
        IMapper mapper,
        IReadableRepository<Case, int> manager,
        IDataRepository<Favorite, (int,int)> favoriteManager,
        ICaseOpenningRepository caseOpenningService,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get all cases
        /// </summary>
        /// <returns>List of all CaseDTO objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Case> caseResult = await manager.GetAllAsyncOld();
            IEnumerable<CaseDTO> caseDTO = mapper.Map<IEnumerable<CaseDTO>>(caseResult);

            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated) return Ok(caseDTO);
            IEnumerable<Favorite> favorites = await authResult.GetByUser(favoriteManager, false);

            foreach (CaseDTO caseDto in caseDTO)
                caseDto.IsFavorite = favorites.Any(fav => fav.CaseId == caseDto.CaseId);

            return Ok(new GetOptions<CaseDTO>(Request, caseDTO));
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
            QueryOptions<Case> options = new QueryOptions<Case>()
                .Before("CaseContents.Skin.Wears.WearType",
                        "CaseContents.Skin.Rarity",
                        "CaseContents.Skin.Item.ItemType");
            Case? result = await manager.GetByIdAsyncNew(id, options);
            if (result == null) return NotFound();
            CaseDTO caseDetailDTO = mapper.Map<CaseDTO>(result);

            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated) return Ok(caseDetailDTO);

            Favorite? favorite = await favoriteManager.GetByIdAsync(
                (authResult.AuthUserId.Value,
                caseDetailDTO.CaseId)
            );
            caseDetailDTO.IsFavorite = favorite != null;
            return Ok(caseDetailDTO);
        }

        /// <summary>
        /// Open one or more cases
        /// </summary>
        /// <returns>MultipleCaseResultDTO object</returns>
        [HttpPost("open")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> OpenCase([FromBody] CaseOpenningDTO caseOpenning)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            Case? caseToOpen = await manager.GetByIdAsyncNew(caseOpenning.CaseId);
            if (caseToOpen == null) return NotFound();
            MultipleCaseResultDTO caseResult;
            try
            {
                caseResult = await caseOpenningService.OpenCaseAsync(
                    caseOpenning,
                    authResult.AuthUserId.Value
                );
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(caseResult);
        }
    }
}
