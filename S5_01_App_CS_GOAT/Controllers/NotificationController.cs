using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/notification")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class NotificationController(
        IMapper mapper,
        IDataRepository<Notification, int, string> dataManager,
        INotificationRepository<Notification> notificationManager,
        IConfiguration configuration
    ) : ControllerBase
    {
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            if (!authResult.IsAdmin)
                return Forbid();

            IEnumerable<Notification> notifications = await dataManager.GetAllAsync(filters, sorts);
            return notifications.Any() ? Ok(notifications) : NotFound();
        }

        [HttpGet("relevant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRelevant([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<Notification> notifications = await notificationManager.GetRelevantAsync(authResult.AuthUserId, filters, sorts);
            return notifications.Any() ? Ok(notifications) : NotFound();
        }

        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDetails(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            Notification? notification = await notificationManager.GetDetailsAsync(id);
            if (notification == null)
                return NotFound();

            return Ok(notification);
        }
    }
}
