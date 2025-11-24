using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/globalNotification")]
    [ApiController]
    public class GlobalNotificationController(
        IMapper mapper,
        IDataRepository<GlobalNotification, int, string> manager
    ) : ControllerBase
    {
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(GlobalNotification globalNotification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await manager.AddAsync(globalNotification);

            return CreatedAtRoute(null, globalNotification);
        }
    }
}