using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/fairRandom")]
    [ApiController]
    public class FairRandomController(
        IMapper mapper,
        IDataRepository<FairRandom, int, string> manager,
        CSGOATDbContext context
        ) : ControllerBase
    {
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FairRandomDTO>>> GetByUser([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            AuthResult authResult = JwtService.Authorized(null);
            int? id = authResult.AuthUserId;

            IEnumerable<FairRandom?> fairRandom = await manager.GetAllAsync(filters, sorts);
            IEnumerable<FairRandom?> userFairRandom = fairRandom.Where(p => p.DependantUserId == id);
            if (userFairRandom == null || !userFairRandom.Any())
                return NotFound();

            IEnumerable<FairRandomDTO?> fairRandomDto = mapper.Map<IEnumerable<FairRandomDTO>>(userFairRandom);
            return Ok(fairRandomDto);
        }
    }
}
