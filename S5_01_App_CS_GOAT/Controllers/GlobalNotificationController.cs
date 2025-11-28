using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/GlobalNotification")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class GlobalNotificationController(
        IMapper mapper,
        IDataRepository<GlobalNotification, int> manager
    ) : ControllerBase
    {
        /// <summary>
        /// Create a new global notification (admin only)
        /// </summary>
        /// <param name="notificationDTO">The NotificationDTO object to create</param>
        /// <returns>The created NotificationDTO object</returns>
        [HttpPost("create")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(NotificationDTO notificationDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            GlobalNotification globalNotification = mapper.Map<GlobalNotification>(notificationDTO);
            await manager.AddAsync(globalNotification);

            NotificationDTO createdNotificationDTO = mapper.Map<NotificationDTO>(globalNotification);
            return CreatedAtAction(null, new { id = globalNotification.NotificationId }, createdNotificationDTO);
        }
    }
}