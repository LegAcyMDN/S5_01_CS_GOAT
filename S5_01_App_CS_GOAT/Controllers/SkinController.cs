using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Skin")]
    [ApiController]
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
            QueryOptions<Case> options = new QueryOptions<Case>()
                .Before("CaseContents.Skin.Wears.WearType",
                        "CaseContents.Skin.Rarity",
                        "CaseContents.Skin.Item");
            Case? _case = await caseManager.GetByIdAsyncNew(caseid, options);
            if (_case == null) return NotFound();

            IEnumerable<SkinDTO> skins = _case.CaseContents.Select(cc => 
                {
                    var skinDto = mapper.Map<SkinDTO>(cc.Skin);
                    skinDto.Weight = cc.Weight;
                    return skinDto;
                });

            return Ok(new GetOptions<SkinDTO>(Request, skins));
        }
    }
}
