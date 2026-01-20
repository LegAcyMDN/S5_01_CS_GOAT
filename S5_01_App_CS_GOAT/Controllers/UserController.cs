using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace S5_01_App_CS_GOAT.Controllers
{
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
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();

            IEnumerable<User> users = await manager.GetAllAsync();
            IEnumerable<UserDTO> dtos = mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(new GetOptions<UserDTO>(Request, dtos));
        }

        /// <summary>
        /// Get the number of connected users
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
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (!auth.IsAdmin && auth.AuthUserId != id)
                return Forbid();

            User? user = await manager.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            UserDTO dto = mapper.Map<UserDTO>(user);
            return Ok(dto);
        }

        /// <summary>
        /// Create a new user account
        /// </summary>
        /// <param name="userDTO">The user data to create</param>
        /// <returns>The created UserDetailDTO object</returns>
        [AllowAnonymous]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

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
        /// Update user details
        /// </summary>
        /// <param name="userDTO">The updated user data</param>
        /// <returns>No content on success</returns>
        [HttpPatch("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO userDTO)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? existing = await manager.GetByIdAsync((int)auth.AuthUserId!);
            if (existing == null)
                return NotFound();

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
        /// Authenticate user with login credentials
        /// </summary>
        /// <param name="loginDTO">The login credentials</param>
        /// <returns>Auth token on successful authentication</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            User? user = await manager.Login(loginDTO);
            if (user == null) return Unauthorized();
            AuthDTO authDTO = await manager.Auth(user, configuration, loginDTO.Remember);
            return Ok(authDTO);
        }

        /// <summary>
        /// Authenticate user with rememberme token
        /// </summary>
        /// <param name="rememberDTO">The login credentials</param>
        /// <returns>Auth token on successful authentication</returns>
        [HttpPost("recall")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Recall([FromBody] TokenDTO rememberDTO)
        {
            User? user = await manager.Recall(rememberDTO);
            if (user == null) return Unauthorized();
            AuthDTO authDTO = await manager.Auth(user, configuration);
            return Ok(authDTO);
        }

        /// <summary>
        /// Request password reset for a user
        /// </summary>
        /// <param name="identifier">The email or login of the user</param>
        /// <param name="url">The URL to send the reset link to (optional)</param>
        /// <param name="code">The reset code (optional)</param>
        /// <param name="preferMail">Whether to prefer email for communication</param>
        /// <returns>Status code indicating the result of the operation</returns>
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
            if (string.IsNullOrEmpty(identifier)) return BadRequest();
            if ((string.IsNullOrEmpty(url) ? 0 : 1) + (string.IsNullOrEmpty(code) ? 0 : 1) != 1)
                return BadRequest();
            if (url != null)
            {
                int response = await manager.StartResetPassword(identifier, url, preferMail);
                return StatusCode(response);
            }
            else
            {
                Tuple<int, string?> response = await manager.EndResetPassword(identifier, code!);
                if (response.Item2 != null) return Ok(response.Item2);
                return StatusCode(response.Item1);
            }
        }

        /// <summary>
        /// Verify user's contact method (SMS or Email)
        /// </summary>
        /// <param name="contact">The contact method to verify ("sms" or "mail")</param>
        /// <param name="code">The verification code (optional)</param>
        /// <returns>Status code indicating the result of the operation</returns>
        [HttpHead("verify/{contact}")]
        [HttpHead("verify/{contact}/{code?}")]
        public async Task<IActionResult> VerifyPhone(string contact, string? code = null)
        {
            if (contact.ToLower() != "sms" && contact.ToLower() != "mail")
                return BadRequest();
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
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
        /// Export user data (GDPR compliance)
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>No content on success</returns>
        [HttpGet("exportdata/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportData(int userId)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (!auth.IsAdmin && auth.AuthUserId != userId)
                return Forbid();

            User? user = await manager.GetByIdAsync(userId);
            if (user == null) return NotFound();

            try
            {
                object? data = await manager.ExportUserDataAsync(user.UserId);
                if (data == null) return NotFound();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Soft delete a user account
        /// </summary>
        /// <returns>No content on success</returns>
        [HttpDelete("delete")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            throw new NotImplementedException();
        }
    }
}
