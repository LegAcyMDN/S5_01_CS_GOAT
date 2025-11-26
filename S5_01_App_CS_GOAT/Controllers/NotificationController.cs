using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Notification")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class NotificationController(
        IMapper mapper,
        IDataRepository<Notification, int, string> manager,
        IDataRepository<GlobalNotification, int, string> globalNotificationManager,
        IDataRepository<UserNotification, int, string> userNotificationManager,
        IConfiguration configuration
    ) : ControllerBase
    {
        /// <summary>
        /// Get all notifications (admin only)
        /// </summary>
        /// <param name="filters">Optional filter parameters</param>
        /// <param name="sorts">Optional sort parameters</param>
        /// <returns>List of all NotificationDTO objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetAll([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            IEnumerable<Notification> notifications = await manager.GetAllAsync(filters, sorts);
            if (!notifications.Any())
                return NotFound();

            IEnumerable<NotificationDTO> notificationsDTO = mapper.Map<IEnumerable<NotificationDTO>>(notifications);
            return Ok(notificationsDTO);
        }

        /// <summary>
        /// Get relevant notifications for the authenticated user
        /// </summary>
        /// <param name="filters">Optional filter parameters</param>
        /// <param name="sorts">Optional sort parameters</param>
        /// <returns>List of NotificationDTO objects for the user (global + user-specific)</returns>
        [HttpGet("relevant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetRelevant([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<GlobalNotification> globalNotifications = await globalNotificationManager.GetAllAsync();
            IEnumerable<UserNotification> userNotifications = await authResult.GetByUser(userNotificationManager, false);

            List<Notification> allRelevantNotifications =
            [
                .. globalNotifications.Cast<Notification>(),
                .. userNotifications.Cast<Notification>(),
            ];

            IEnumerable<NotificationDTO> notificationsDTO = mapper.Map<IEnumerable<NotificationDTO>>(allRelevantNotifications);
            return Ok(notificationsDTO);
        }

        /// <summary>
        /// Get notification details by ID
        /// </summary>
        /// <param name="id">The ID of the notification</param>
        /// <returns>NotificationDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationDTO>> GetDetails(int id)
        {
            Notification? notification = await manager.GetByIdAsync(id);
            if (notification == null)
                return NotFound();

            NotificationDTO notificationDTO = mapper.Map<NotificationDTO>(notification);
            return Ok(notificationDTO);
        }
    }
}
