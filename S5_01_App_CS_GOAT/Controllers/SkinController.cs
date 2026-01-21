using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages skin (item variant) information and retrieval
    /// </summary>
    [Route("api/Skin")]
    [ApiController]
    public class SkinController(
        IMapper mapper,
        IReadableRepository<Case, int> caseManager,
        IReadableRepository<Skin, int> skinManager
        ) : ControllerBase
    {
        /// <summary>
        /// Get all available skins with wear variations
        /// </summary>
        /// <returns>List of SkinDTO objects with all wear classes</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            QueryOptions<Skin> options = new QueryOptions<Skin>()
                .Before(s => s.Wears, s => s.Rarity, s => s.Item);
            IEnumerable<Skin> skinsEntity = await skinManager.GetAllAsync(options);
            IEnumerable<SkinDTO> skins = mapper.Map<IEnumerable<SkinDTO>>(skinsEntity);
            return Ok(new GetOptions<SkinDTO>(Request, skins));
        }

        /// <summary>
        /// Get skins available in a specific case with drop weights
        /// </summary>
        /// <param name="caseid">The ID of the case</param>
        /// <returns>List of SkinDTO objects for the case with drop probabilities</returns>
        [HttpGet("bycase/{caseid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCase(int caseid)
        {
            QueryOptions<Case> options = new QueryOptions<Case>()
                .Before("CaseContents.Skin.Wears",
                        "CaseContents.Skin.Rarity",
                        "CaseContents.Skin.Item");
            Case? _case = await caseManager.GetByIdAsync(caseid, options);
            if (_case == null)
            {
                return NotFound();
            }

            IEnumerable<SkinDTO> skins = _case.CaseContents.Select(cc =>
                {
                    SkinDTO skinDto = mapper.Map<SkinDTO>(cc.Skin);
                    skinDto.Weight = cc.Weight;
                    return skinDto;
                });

            return Ok(new GetOptions<SkinDTO>(Request, skins));
        }
    }
}
