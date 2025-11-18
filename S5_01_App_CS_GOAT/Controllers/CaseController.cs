namespace S5_01_App_CS_GOAT.Controllers
{
    using AutoMapper;
    using global::S5_01_App_CS_GOAT.Models.EntityFramework;
    using global::S5_01_App_CS_GOAT.Models.Repository;
    using Microsoft.AspNetCore.Mvc;

    namespace S5_01_App_CS_GOAT.Controllers
    {

        [Route("api/Case")]
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
                return result == null ? NotFound() : Ok(result);
            }

            [HttpDelete("remove/{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Delete(int id)
            {
                ActionResult<Case?> caseResult = await manager.GetByIdAsync(id);
                if (caseResult.Value == null)
                    return NotFound();
                await manager.DeleteAsync(caseResult.Value);
                return NoContent();
            }

            [HttpGet("all")]
            [ProducesResponseType(StatusCodes.Status200OK)]
            public async Task<ActionResult<IEnumerable<Case>>> GetAll()
            {
                IEnumerable<Case?> caseResult = await manager.GetAllAsync();
                if (caseResult == null || !caseResult.Any())
                    return NotFound();
                return Ok(caseResult);
            }

            [HttpPost("create")]
            [ProducesResponseType(StatusCodes.Status201Created)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> Create(Case caseItem)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await manager.AddAsync(caseItem);

                return CreatedAtAction("Get", new { id = caseItem.CaseId }, caseItem);
            }

            [HttpPut("update/{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status400BadRequest)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Update(int id, Case Case)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Case? caseToUpdate = await manager.GetByIdAsync(id);

                if (caseToUpdate == null)
                {
                    return NotFound();
                }
                await manager.UpdateAsync(caseToUpdate, Case);

                return NoContent();
            }
        }
    }



}
