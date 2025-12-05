using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Wear")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class WearController(
        IMapper mapper,
        IReadableRepository<Wear, int> manager
    ) : ControllerBase
    {
        /// <summary>
        /// Get 3D model by wear ID
        /// </summary>
        /// <param name="wearId">The ID of the wear</param>
        /// <returns>ModelDTO object for the wear</returns>
        [HttpGet("get3dmodel/{wearId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get3dModelByWear(int wearId)
        {
            Wear? wear = await manager.GetByIdAsync(wearId, "Skin.Item");
            if (wear == null) return NotFound();
            ModelDTO modelDto = mapper.Map<ModelDTO>(wear);
            return Ok(modelDto);
        }
    }
}