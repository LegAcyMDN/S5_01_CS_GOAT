using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages global system notifications (admin operations)
    /// </summary>
    [Route("api/GlobalNotification")]
    [ApiController]
    [SetThreadPrincipal]
    public class GlobalNotificationController(
        IMapper mapper,
        IDataRepository<GlobalNotification, int> manager,
        ITypeRepository<NotificationType> typeManager,
        IConfiguration configuration
    ) : ControllerBase
    {
        /// <summary>
        /// Create a new global notification (admin only)
        /// </summary>
        /// <param name="notificationDTO">The NotificationDTO object to create</param>
        /// <returns>The created NotificationDTO object</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(NotificationDTO notificationDTO)
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

            NotificationType? notificationType = typeManager.GetTypeByName(notificationDTO.NotificationTypeName);
            if (notificationType == null)
            {
                return BadRequest($"Invalid notification type: {notificationDTO.NotificationTypeName}");
            }

            GlobalNotification globalNotification = mapper.Map<GlobalNotification>(notificationDTO);
            globalNotification.NotificationTypeId = notificationType.NotificationTypeId;
            _ = await manager.AddAsync(globalNotification);

            NotificationDTO createdNotificationDTO = mapper.Map<NotificationDTO>(globalNotification);
            return CreatedAtAction(null, new { id = globalNotification.NotificationId }, createdNotificationDTO);
        }
    }
}