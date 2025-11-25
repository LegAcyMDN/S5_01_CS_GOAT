using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/NotificationSetting")]
    [ApiController]
    public class NotificationSettingController(
        IDataRepository<NotificationSetting, int, string> manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get notification settings for the authenticated user
        /// </summary>
        /// <returns>List of notification settings for the user</returns>
        [HttpGet("ByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            int userId = authResult.AuthUserId;
            IEnumerable<NotificationSetting> settings = await manager.GetAllAsync();
            IEnumerable<NotificationSetting> userSettings = settings.Where(ns => ns.UserId == userId);

            return Ok(userSettings);
        }

        /// <summary>
        /// Update notification settings for the authenticated user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="notificationTypeId">The ID of the notification type</param>
        /// <param name="patchData">The patch data with OnSite, ByEmail, ByPhone flags</param>
        /// <returns>No content on success</returns>
        [HttpPatch("Update/{userId}/{notificationTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int userId, int notificationTypeId, [FromBody] Dictionary<string, object> patchData)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            if (userId != authResult.AuthUserId)
                return Forbid();

            NotificationSetting? setting = await manager.GetByIdsAsync(userId, notificationTypeId);
            if (setting == null)
                return NotFound();

            await manager.PatchAsync(setting, patchData);
            return NoContent();
        }
    }
}
