using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Case")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class CaseController(
        IMapper mapper,
        IDataRepository<Case, int, string> manager,
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
            IEnumerable<Case?> caseResult = await manager.GetAllAsync();

            // TODO: Use the auth user id to detect if case is user's favorite
            IEnumerable<CaseDTO?> caseDTO = mapper.Map<IEnumerable<CaseDTO>>(caseResult);
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
            return Ok(mapper.Map<CaseDetailDTO>(result));
        }
    }
}
