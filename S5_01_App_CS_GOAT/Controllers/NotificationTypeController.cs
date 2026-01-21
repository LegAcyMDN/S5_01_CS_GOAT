using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/NotificationType")]
    [ApiController]
    public class NotificationTypeController(
        IMapper mapper,
        ITypeRepository<NotificationType> manager
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
