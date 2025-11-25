using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using System;
using System.Collections.Generic;

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
   

        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FairRandomDTO>>> GetByUser()
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
