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
        IReadableRepository<Case, int> caseManager
        ) : ControllerBase
    {
        /// <summary>
        /// Get skins by case ID
        /// </summary>
        /// <param name="caseid">The ID of the case</param>
        /// <returns>List of SkinDTO objects for the case</returns>
        [HttpGet("bycase/{caseid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCase(int caseid)
        {
            Case? _case = await caseManager.GetByIdAsync(caseid,
                "CaseContents.Skin.Rarity",
                "CaseContents.Skin.PriceHistories.WearType",
                "CaseContents.Skin.Wears.WearType",
                "CaseContents.Skin.Item"
            );
            if (_case == null) return NotFound();

            IEnumerable<SkinDTO> skins = _case.CaseContents.Select(cc => 
                {
                    var skinDto = mapper.Map<SkinDTO>(cc.Skin);
                    skinDto.Weight = cc.Weight;
                    return skinDto;
                });

            return Ok(skins);
        }
    }
}
