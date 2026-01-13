using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/steam")]
    [ApiController]
    [SetThreadPrincipal]
    public class SteamLoginController(
        IUserRepository userRepository,
        IConfiguration configuration
    ) : ControllerBase
    {

#if DEBUG
        private static string URL = "https://localhost:7030";
#else
        private static string URL = "https://blazorcsgoat-a4gke7edayahgcef.eastus-01.azurewebsites.net";
#endif
        /// <summary>
        /// Initiate Steam OAuth login flow
        /// </summary>
        /// <param name="linkUserId">Optional user ID to link Steam account to existing user</param>
        /// <returns>Redirect to Steam authentication</returns>
        [HttpGet("login")]
        public IActionResult SteamLogin([FromQuery] string? linkUserId = null)
        {
            string? redirectUri = Url.Action(nameof(Callback));

            if (!string.IsNullOrEmpty(linkUserId))
            {
                redirectUri += $"?linkUserId={linkUserId}";
            }

            AuthenticationProperties properties = new AuthenticationProperties 
            { 
                RedirectUri = redirectUri,
                Items =
                {
                    { "returnUrl", URL }
                }
            };
            
            if (!string.IsNullOrEmpty(linkUserId))
            {
                properties.Items["linkUserId"] = linkUserId;
            }
            
            return Challenge(properties, "Steam");
        }

        /// <summary>
        /// Handle Steam OAuth callback
        /// </summary>
        /// <returns>Redirect to Blazor app with authentication result</returns>
        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            AuthenticateResult? authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            if (!authenticateResult.Succeeded)
                return Redirect($"{URL}?error=authentication_failed");

            string? steamId = authenticateResult.Principal?.FindFirst("steamid")?.Value;
            
            if (string.IsNullOrEmpty(steamId))
                return Redirect($"{URL}?error=missing_user_id");

            string? existingUserId = Request.Query["linkUserId"].ToString();
            
            if (!string.IsNullOrEmpty(existingUserId) && int.TryParse(existingUserId, out int userIdToLink))
            {
                User? existingUser = await userRepository.GetByIdAsync(userIdToLink);
                if (existingUser == null)
                    return Redirect($"{URL}?error=user_not_found");

                User? steamUser = await userRepository.GetBySteamIdAsync(steamId);
                if (steamUser != null && steamUser.UserId != userIdToLink)
                    return Redirect($"{URL}/profile?error=steam_already_linked");

                existingUser.SteamId = steamId;
                await userRepository.UpdateAsync(existingUser);

                return Redirect($"{URL}/profile?success=steam_linked");
            }
            else
            {
                User? user = await userRepository.GetBySteamIdAsync(steamId);
                
                if (user == null)
                    return Redirect($"{URL}?error=user_not_found");

                AuthDTO? authDTO = await userRepository.Auth(user, configuration, remember: 7);

                return Redirect($"{URL}/auth-callback?token={authDTO.JwtToken}&userId={authDTO.UserId}&displayName={Uri.EscapeDataString(user.DisplayName)}");
            }
        }
        
        /// <summary>
        /// Logout from Steam authentication
        /// </summary>
        /// <returns>Redirect to home page</returns>
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(URL);
        }

        /// <summary>
        /// Unlink Steam account from user
        /// </summary>
        /// <returns>No content on success</returns>
        [HttpPatch("unlink")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UnlinkSteam()
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();

            User? user = await userRepository.GetByIdAsyncNew((int)auth.AuthUserId!);
            if (user == null)
                return NotFound();

            if (string.IsNullOrEmpty(user.SteamId))
                return BadRequest("Aucun compte Steam n'est lié à cet utilisateur.");

            user.SteamId = null;
            await userRepository.UpdateAsync(user);

            return NoContent();
        }
    }
}