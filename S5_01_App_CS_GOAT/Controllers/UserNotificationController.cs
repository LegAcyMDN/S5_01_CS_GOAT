using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;


namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages user-specific notifications (admin operations)
    /// </summary>
    [Route("api/UserNotification")]
    [ApiController]
    [SetThreadPrincipal]
    public class UserNotificationController(
        IMapper mapper,
        IDataRepository<UserNotification, int> manager,
        ITypeRepository<NotificationType> typeManager
        , IConfiguration configuration
    ) : ControllerBase
    {
        /// <summary>
        /// Create a new user-specific notification (admin only)
        /// </summary>
        /// <param name="notificationDto">The notification data to create</param>
        /// <returns>The created NotificationDTO object</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(NotificationDTO notificationDto)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            NotificationType? notificationType = typeManager.GetTypeByName(notificationDto.NotificationTypeName);
            if (notificationType == null)
            {
                return BadRequest($"Invalid notification type: {notificationDto.NotificationTypeName}");
            }

            UserNotification userNotification = mapper.Map<UserNotification>(notificationDto);
            userNotification.NotificationTypeId = notificationType.NotificationTypeId;

            _ = await manager.AddAsync(userNotification);

            NotificationDTO resultDto = mapper.Map<NotificationDTO>(userNotification);
            return CreatedAtRoute(null, resultDto);
        }
    }
}