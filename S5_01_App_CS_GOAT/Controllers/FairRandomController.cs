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
        CSGOATDbContext context
        ) : ControllerBase
    {
   

        [HttpGet("ByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FairRandomDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.Authorized(null);
            int? id = authResult.AuthUserId;
          
            IEnumerable<FairRandom?> fairRandom = await manager.GetAllAsync();
            IEnumerable<FairRandom?> userfairRandom = fairRandom.Where(p => p.DependantUserId == id);
            if (userfairRandom == null || !userfairRandom.Any())
                return NotFound();

            IEnumerable<FairRandomDTO?> fairRandomDto = mapper.Map<IEnumerable<FairRandomDTO>>(userfairRandom);
            return Ok(fairRandomDto);
        }

       
        }
    }
