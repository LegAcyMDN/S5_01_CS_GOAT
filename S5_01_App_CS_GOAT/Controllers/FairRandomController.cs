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
        CSGOATDbContext context,
        IConfiguration configuration
        ) : ControllerBase
    {
   

        [HttpGet("ByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FairRandomDTO>>> GetByUser([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<FairRandom> fairRandoms = await authResult.GetByUser(manager, false);
            if (!fairRandoms.Any()) return NotFound();

            IEnumerable<FairRandomDTO> userBansDTO = mapper.Map<IEnumerable<FairRandomDTO>>(fairRandoms);
            return Ok(userBansDTO);
        }

       
    }
}
