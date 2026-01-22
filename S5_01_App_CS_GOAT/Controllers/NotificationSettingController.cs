using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages notification preferences for users (channel and type configuration)
    /// </summary>
    [Route("api/NotificationSetting")]
    [ApiController]
    [SetThreadPrincipal]
    public class NotificationSettingController(
        IDataRepository<NotificationSetting, (int, int)> manager,
        ITypeRepository<NotificationType> typeRepository,
        IConfiguration configuration,
        IMapper mapper) : ControllerBase
    {
        /// <summary>
        /// Get notification settings for the authenticated user
        /// </summary>
        /// <returns>List of notification settings for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            QueryOptions<NotificationSetting> queryOptions = new QueryOptions<NotificationSetting>()
                .Before(ns => ns.NotificationType);
            IEnumerable<NotificationSetting> userSettings = await authResult.GetByUser(manager, false, queryOptions);
            IEnumerable<NotificationSettingDTO> dtoSettings = mapper.Map<IEnumerable<NotificationSettingDTO>>(userSettings);

            return Ok(dtoSettings);
        }

        /// <summary>
        /// Update notification settings for authenticated user
        /// </summary>
        /// <param name="notificationTypeName">The name of the notification type</param>
        /// <param name="patchData">The patch data with OnSite, ByEmail, ByPhone flags</param>
        /// <returns>No content on success</returns>
        [HttpPatch("update/{notificationTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(string notificationTypeName, [FromBody] Dictionary<string, object> patchData)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId = authResult.AuthUserId!.Value;

            NotificationType? notificationType = typeRepository.GetTypeByName(notificationTypeName);
            if (notificationType == null)
            {
                return NotFound();
            }

            NotificationSetting? setting = await manager.GetByIdAsync((userId, notificationType.NotificationTypeId));
            if (setting == null)
            {
                return NotFound();
            }

            await manager.PatchAsync(setting, patchData);
            return NoContent();
        }
    }
}
