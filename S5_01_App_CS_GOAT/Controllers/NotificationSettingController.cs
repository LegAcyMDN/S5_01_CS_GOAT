using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/NotificationSetting")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class NotificationSettingController(
        IDataRepository<NotificationSetting, (int,int)> manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get notification settings for the authenticated user
        /// </summary>
        /// <returns>List of notification settings for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationSetting>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<NotificationSetting> userSettings = await authResult.GetByUser(manager, false);
            
            return Ok(userSettings);
        }

        /// <summary>
        /// Update notification settings for the authenticated user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="notificationTypeId">The ID of the notification type</param>
        /// <param name="patchData">The patch data with OnSite, ByEmail, ByPhone flags</param>
        /// <returns>No content on success</returns>
        [HttpPatch("update/{userId}/{notificationTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int userId, int notificationTypeId, [FromBody] Dictionary<string, object> patchData)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (userId != authResult.AuthUserId)
                return Forbid();

            NotificationSetting? setting = await manager.GetByIdAsync((userId, notificationTypeId));
            if (setting == null) return NotFound();

            await manager.PatchAsync(setting, patchData);
            return NoContent();
        }
    }
}
