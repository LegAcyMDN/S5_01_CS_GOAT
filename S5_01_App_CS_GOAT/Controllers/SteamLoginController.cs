using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [ApiController]
    [Route("api/steam")]
    public class SteamLoginController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public SteamLoginController(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        [HttpGet("login")]
        public IActionResult SteamLogin()
        {
            var properties = new AuthenticationProperties 
            { 
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                    { "returnUrl", "https://localhost:7030" }
                }
            };
            
            return Challenge(properties, "Steam");
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback()
        {
            // Authenticate from cookie
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            if (!authenticateResult.Succeeded)
            {
                Console.WriteLine("❌ Authentication failed in callback");
                return Redirect("https://localhost:7030?error=authentication_failed");
            }

            // Get user info from claims
            var userId = authenticateResult.Principal?.FindFirst("user_id")?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("❌ No user_id claim found");
                return Redirect("https://localhost:7030?error=missing_user_id");
            }

            // Get user from database
            var user = await _userRepository.GetByIdAsync(int.Parse(userId));
            
            if (user == null)
            {
                Console.WriteLine("❌ User not found in database");
                return Redirect("https://localhost:7030?error=user_not_found");
            }

            // Generate JWT token
            var authDTO = await _userRepository.Auth(user, _configuration, remember: 7);

            Console.WriteLine($"✅ Redirecting to Blazor with token for user {user.UserId}");

            // Redirect to Blazor with JWT token
            return Redirect($"https://localhost:7030/auth-callback?token={authDTO.JwtToken}&userId={authDTO.UserId}&displayName={Uri.EscapeDataString(user.DisplayName)}");
        }
        
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("https://localhost:7030");
        }
    }
}