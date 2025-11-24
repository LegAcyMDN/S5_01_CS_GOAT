using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Wear")]
    [ApiController]
    public class WearController(
        IMapper mapper,
        IDataRepository<Wear, int, string> manager
    ) : ControllerBase
    {

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Wear?>>> GetAll()
        {
            IEnumerable<Wear?> wear = await manager.GetAllAsync();
            if (wear == null || !wear.Any())
                return NotFound();
            return Ok(wear);
        }
    }
    }