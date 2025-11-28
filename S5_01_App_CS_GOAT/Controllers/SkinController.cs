using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Skin")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class SkinController(
        IMapper mapper,
        ISkinRelatedRepository<Skin> manager,
        IDataRepository<Case, int> caseManager
        ) : ControllerBase
    {
        /// <summary>
        /// Get skins by case ID
        /// </summary>
        /// <param name="id">The ID of the case</param>
        /// <returns>List of SkinDTO objects for the case</returns>
        [HttpGet("bycase/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<SkinDTO>>> GetByCase(int id)
        {
            Case? caseExists = await caseManager.GetByIdAsync(id);
            if (caseExists == null) return NotFound();

            IEnumerable<Skin?> skins = await manager.GetByCase(id);
            IEnumerable<SkinDTO> skinDto = mapper.Map<IEnumerable<SkinDTO>>(skins);
            return Ok(skinDto);
        }
    }
}
