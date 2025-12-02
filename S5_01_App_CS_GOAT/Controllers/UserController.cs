using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/User")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class UserController(
        IUserRepository manager,
        IMapper mapper,
        IConfiguration configuration
    ) : ControllerBase
    {

        /// <summary>
        /// Get all users (admin only)
        /// </summary>
        /// <returns>List of all UserDetailDTO objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<User> users = await manager.GetAllAsync();
            IEnumerable<UserDTO> dtos = mapper.Map<IEnumerable<UserDTO>>(users);
            return Ok(dtos);
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
        /// <param name="id">The ID of the user to update</param>
        /// <param name="userDTO">The updated user data</param>
        /// <returns>No content on success</returns>
        [HttpPatch("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDTO userDTO)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (auth.AuthUserId != id)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User? existing = await manager.GetByIdAsync(id);
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
        public async Task<IActionResult> Recall([FromBody] RememberDTO rememberDTO)
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
        /// <returns>No content (always returns success for security)</returns>
        [HttpHead("resetpassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ResetPassword([FromQuery] string identifier)
        {
            if (string.IsNullOrEmpty(identifier))
                return BadRequest();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Verify user's phone number with code
        /// </summary>
        /// <param name="code">The verification code</param>
        /// <returns>No content on success</returns>
        [HttpHead("verifyphone")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyPhone([FromQuery] string code)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Verify user's email address with code
        /// </summary>
        /// <param name="code">The verification code</param>
        /// <returns>No content on success</returns>
        [HttpHead("verifymail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyMail([FromQuery] string code)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();

            throw new NotImplementedException();
        }

        /// <summary>
        /// Export user data (GDPR compliance)
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <returns>No content on success</returns>
        [HttpHead("exportdata")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExportData([FromQuery] int id)
        {
            AuthResult auth = JwtService.JwtAuth(configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (!auth.IsAdmin && auth.AuthUserId != id)
                return Forbid();

            User? user = await manager.GetByIdAsync(id);
            if (user == null) return NotFound();

            throw new NotImplementedException();
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
