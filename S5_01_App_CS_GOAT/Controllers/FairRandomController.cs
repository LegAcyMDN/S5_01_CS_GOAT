using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
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
   

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FairRandomDTO>>> GetAll()
        {
            IEnumerable<FairRandom?> fairRandom = await manager.GetAllAsync();
            if (fairRandom == null || !fairRandom.Any())
                return NotFound();

            IEnumerable<FairRandomDTO?> fairRandomDto = mapper.Map<IEnumerable<FairRandomDTO>>(fairRandom);
            return Ok(fairRandomDto);
        }

       
        }
    }
