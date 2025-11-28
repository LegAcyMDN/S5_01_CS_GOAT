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
        INotificationRelatedRepository<int?> manager
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

            int? notificationTypeId = await manager.GetNotificationTypeIdByNameAsync(notificationDto.NotificationTypeName);

            if (notificationTypeId == null)
            {
                return BadRequest($"Notification type '{notificationDto.NotificationTypeName}' not found.");
            }

            UserNotification userNotification = mapper.Map<UserNotification>(notificationDto);

            await manager.AddAsync(userNotification);

            NotificationDTO resultDto = mapper.Map<NotificationDTO>(userNotification);
            return CreatedAtRoute(null, resultDto);
        }
    }
}