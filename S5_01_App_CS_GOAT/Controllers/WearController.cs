using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
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
        IWearRelatedRepository<Wear> manager
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
        public async Task<ActionResult<ModelDTO>> Get3dModelByWear(int wearId)
        {
            var wear = await manager.GetBy3dModelAsync(wearId);
            if (wear == null || !wear.Any())
                return NotFound();

            var firstWear = wear.First();
            ModelDTO modelDto = mapper.Map<ModelDTO>(firstWear);
            return Ok(modelDto);
        }
    }
}