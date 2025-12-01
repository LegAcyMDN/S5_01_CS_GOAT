using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/UserNotification")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class UserNotificationController(
        IMapper mapper,
        IDataRepository<UserNotification, int> manager,
        ITypeRepository<NotificationType> typeManager
    ) : ControllerBase
    {
        /// <summary>
        /// Create a new user notification (admin only)
        /// </summary>
        /// <param name="notificationDto">The notification data to create</param>
        /// <returns>The created NotificationDTO object</returns>
        [HttpPost("create")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(NotificationDTO notificationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            NotificationType? notificationType = await typeManager.GetTypeByNameAsync(notificationDto.NotificationTypeName);
            if (notificationType == null)
                return BadRequest($"Invalid notification type: {notificationDto.NotificationTypeName}");
            UserNotification userNotification = mapper.Map<UserNotification>(notificationDto);
            userNotification.NotificationTypeId = notificationType.NotificationTypeId;

            await manager.AddAsync(userNotification);

            NotificationDTO resultDto = mapper.Map<NotificationDTO>(userNotification);
            return CreatedAtRoute(null, resultDto);
        }
    }
}