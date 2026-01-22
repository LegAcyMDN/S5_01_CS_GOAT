using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages user account operations including authentication, profile management, and verification
    /// </summary>
    [Route("api/User")]
    [ApiController]
    [SetThreadPrincipal]
    public class UserController(
        IUserRepository manager,
        ISendingRepository sendingManager,
        IMapper mapper,
        IConfiguration configuration
    ) : ControllerBase
    {

        /// <summary>
        /// Get all users (admin only)
        /// </summary>
        /// <returns>List of all UserDetailDTO objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            IEnumerable<User> users = await manager.GetAllAsync();
            IEnumerable<UserDTO> dtos = mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(new GetOptions<UserDTO>(Request, dtos));
        }

        /// <summary>
        /// Get count of currently connected users (active in last 15 minutes)
        /// </summary>
        /// <returns>Count of connected users</returns>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCount()
        {
            QueryOptions<User> options = new QueryOptions<User>()
                .Before(u => u.LastLogin > DateTime.UtcNow.AddMinutes(-15));
            IEnumerable<User> users = await manager.GetAllAsync(options);
            return Ok(users.Count());
        }

        /// <summary>
        /// Get user details by ID
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>UserDetailDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!auth.IsAdmin && auth.AuthUserId != id)
            {
                return Forbid();
            }

            User? user = await manager.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            UserDTO dto = mapper.Map<UserDTO>(user);
            return Ok(dto);
        }

        /// <summary>
        /// Create a new user account with automatic authentication
        /// </summary>
        /// <param name="userDTO">The user data to create</param>
        /// <returns>AuthDTO with JWT token for the new account</returns>
        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO userDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                User newUser = await manager.CreateUser(userDTO);
                AuthDTO authDTO = await manager.Auth(newUser, configuration, userDTO.Remember);
                return CreatedAtAction(nameof(Get), new { id = newUser.UserId }, authDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update authenticated user details including email, phone, 2FA, and password
        /// </summary>
        /// <param name="userDTO">The updated user data</param>
        /// <returns>No content on success</returns>
        [HttpPatch("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO userDTO)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User? existing = await manager.GetByIdAsync((int)auth.AuthUserId!);
            if (existing == null)
            {
                return NotFound();
            }

            try
            {
                await manager.UpdateUserDetails(existing, userDTO);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Authenticate user with login credentials (login, email, or phone)
        /// </summary>
        /// <param name="loginDTO">The login credentials</param>
        /// <returns>AuthDTO with JWT token on successful authentication</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            User? user = await manager.Login(loginDTO);
            if (user == null)
            {
                return Unauthorized();
            }

            AuthDTO authDTO = await manager.Auth(user, configuration, loginDTO.Remember);
            return Ok(authDTO);
        }

        /// <summary>
        /// Authenticate user with remember token for persistent sessions
        /// </summary>
        /// <param name="rememberDTO">The remember token credentials</param>
        /// <returns>AuthDTO with new JWT token on successful recall</returns>
        [HttpPost("recall")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Recall([FromBody] TokenDTO rememberDTO)
        {
            User? user = await manager.Recall(rememberDTO);
            if (user == null)
            {
                return Unauthorized();
            }

            AuthDTO authDTO = await manager.Auth(user, configuration);
            return Ok(authDTO);
        }

        /// <summary>
        /// Request password reset or complete reset with reset code
        /// </summary>
        /// <param name="identifier">User identifier (login, email, or phone)</param>
        /// <param name="url">Reset link base URL (for step 1: request reset)</param>
        /// <param name="code">Reset verification code (for step 2: complete reset)</param>
        /// <param name="preferMail">Whether to prefer email over SMS for reset link</param>
        /// <returns>HTTP status codes or new temporary password on completion</returns>
        [HttpGet("resetpassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResetPassword(
            [FromQuery] string identifier,
            [FromQuery] string? url = null,
            [FromQuery] string? code = null,
            [FromQuery] bool preferMail = true)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                return BadRequest();
            }

            if ((string.IsNullOrEmpty(url) ? 0 : 1) + (string.IsNullOrEmpty(code) ? 0 : 1) != 1)
            {
                return BadRequest();
            }

            if (url != null)
            {
                int response = await manager.StartResetPassword(identifier, url, preferMail);
                return StatusCode(response);
            }
            else
            {
                Tuple<int, string?> response = await manager.EndResetPassword(identifier, code!);
                return response.Item2 != null ? Ok(response.Item2) : StatusCode(response.Item1);
            }
        }

        /// <summary>
        /// Verify user's contact method (SMS or Email) - request code or verify code
        /// </summary>
        /// <param name="contact">Contact method type: "sms" or "mail"</param>
        /// <param name="code">Verification code (omit to request new code)</param>
        /// <returns>HTTP status codes indicating verification result</returns>
        [HttpHead("verify/{contact}")]
        [HttpHead("verify/{contact}/{code?}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> VerifyPhone(string contact, string? code = null)
        {
            if (contact.ToLower() != "sms" && contact.ToLower() != "mail")
            {
                return BadRequest();
            }

            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            User user = (await manager.GetByIdAsync((int)auth.AuthUserId!))!;
            int response = (contact, code) switch
            {
                ("sms", not null) => await sendingManager.VerifySmsAsync(user, code),
                ("mail", not null) => await sendingManager.VerifyMailAsync(user, code),
                ("sms", null) => await sendingManager.NewCodeSmsAsync(user),
                ("mail", null) => await sendingManager.NewCodeMailAsync(user),
                _ => throw new NotImplementedException()
            };
            return StatusCode(response);
        }

        /// <summary>
        /// Export user data for GDPR compliance (data portability)
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>Complete user data export including inventory and transactions</returns>
        [HttpGet("exportdata/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportData(int userId)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!auth.IsAdmin && auth.AuthUserId != userId)
            {
                return Forbid();
            }

            User? user = await manager.GetByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                object? data = await manager.ExportUserDataAsync(user.UserId);
                return data == null ? NotFound() : Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Soft delete the authenticated user account (GDPR right to be forgotten)
        /// </summary>
        /// <returns>No content on success</returns>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Delete()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            return !authResult.IsAuthenticated ? (IActionResult)Unauthorized() : throw new NotImplementedException();
        }
    }
}
