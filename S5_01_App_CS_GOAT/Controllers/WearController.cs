using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Mapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/wear")]
    [ApiController]
    public class WearController(
        IMapper mapper,
        IWearRelatedRepository<Wear> manager
    ) : ControllerBase
    {

        [HttpGet("By3dModel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ModelDTO>>> GetBy3dModel(int id)
        {
            IEnumerable<Wear?> wear = await manager.GetBy3dModelAsync(id);
            if (wear == null || !wear.Any())
                return NotFound();
            IEnumerable<ModelDTO> modelDto = mapper.Map<IEnumerable<ModelDTO>>(wear);
            return Ok(modelDto);
        }
    }
}