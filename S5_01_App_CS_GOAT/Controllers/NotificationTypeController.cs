using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/NotificationType")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class NotificationTypeController(
        IMapper mapper,
        IDataRepository<NotificationType, int> manager
    ) : ControllerBase
    {
        /// <summary>
        /// Get all notification types
        /// </summary>
        /// <returns>List of all NotificationTypeDTO objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<NotificationType> notificationTypes = await manager.GetAllAsync();
            IEnumerable<NotificationTypeDTO> notificationTypesDTO = mapper.Map<IEnumerable<NotificationTypeDTO>>(notificationTypes);
            return Ok(notificationTypesDTO);
        }
    }
}
