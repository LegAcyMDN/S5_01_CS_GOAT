using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;


namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/UserNotification")]
    [ApiController]
    public class UserNotificationController(
        IMapper mapper,
        INotificationRelatedRepository<int?> manager,
        IConfiguration configuration,
        CSGOATDbContext context
    ) : ControllerBase
    {
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(NotificationDTO notificationDto)
        {

            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            if (!authResult.IsAdmin)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            if(string.IsNullOrEmpty(notificationDto.NotificationTypeName))
            {
                return BadRequest("Le nom du type de notification est requis.");
            }


            int? notificationTypeId = await manager.GetNotificationTypeIdByNameAsync(notificationDto.NotificationTypeName);

            if (notificationTypeId == null )
            {
                return BadRequest($"Type de notification '{notificationDto.NotificationTypeName}' introuvable.");
            }


            UserNotification userNotification = mapper.Map<UserNotification>(notificationDto);

            await manager.AddAsync(userNotification);

            NotificationDTO resultDto = mapper.Map<NotificationDTO>(userNotification);
            return CreatedAtRoute(null, resultDto);

        }
    }
}