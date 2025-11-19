namespace S5_01_App_CS_GOAT.Controllers
{
    using global::AutoMapper;
    using global::S5_01_App_CS_GOAT.DTO;
    using global::S5_01_App_CS_GOAT.Models.EntityFramework;
    using global::S5_01_App_CS_GOAT.Models.Repository;
    using Microsoft.AspNetCore.Mvc;
    namespace S5_01_App_CS_GOAT.Controllers
    {

        [Route("api/Cases")]
        [ApiController]
        public class CaseController(
             IMapper mapper,
            IDataRepository<Case, int, string> manager,
            CSGOATDbContext context
            ) : ControllerBase
        {
            [HttpGet("details/{id}")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Get(int id)
            {
                Case? result = await manager.GetByIdAsync(id);
                return result == null ? NotFound() : Ok(mapper.Map<CaseDTO>(result));
            }


            [HttpGet("all")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<ActionResult<IEnumerable<CaseDTO>>> GetAll()
            {
                IEnumerable<Case?> caseResult = await manager.GetAllAsync();
                if (caseResult == null || !caseResult.Any())
                    return NotFound();
                IEnumerable<CaseDTO?> caseDto = mapper.Map<IEnumerable<CaseDTO>>(caseResult);
                return Ok(caseDto);
            }
        }



    }
}
