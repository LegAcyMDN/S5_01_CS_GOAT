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
    public class SkinController(
        IMapper mapper,
        ISkinRelatedRepository<Skin> manager,
        CSGOATDbContext context
        ) : ControllerBase
    {


        [HttpGet("ByCase")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SkinDTO>>> GetByCase(int id)
        {
            IEnumerable<Skin?> skins = await manager.getByCase(id);

            if (skins == null || !skins.Any())
                return NotFound();

            IEnumerable<SkinDTO> skinDto = mapper.Map<IEnumerable<SkinDTO>>(skins);
            return Ok(skinDto);
        }


    }
}
