using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.DataManager;
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
        IDataRepository<User, int, string> manager,
        IMapper mapper,
        IConfiguration configuration
    ) : ControllerBase
    {
        private readonly UserManager _manager = (UserManager)manager;
        private readonly IMapper _mapper = mapper;
        private readonly IConfiguration _configuration = configuration;

        /// <summary>
        /// Get all users (admin only)
        /// </summary>
        /// <returns>List of all UserDetailDTO objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<User> users = await _manager.GetAllForAdminAsync();
            IEnumerable<UserDetailDTO> dtos = _mapper.Map<IEnumerable<UserDetailDTO>>(users);
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
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (!auth.IsAdmin && auth.AuthUserId != id)
                return Forbid();

            var user = await _manager.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            var dto = _mapper.Map<UserDetailDTO>(user);
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
        public async Task<IActionResult> Create([FromBody] UserDetailDTO userDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entity = _mapper.Map<User>(userDTO);
            await _manager.AddAsync(entity);
            var createdDto = _mapper.Map<UserDetailDTO>(entity);
            return CreatedAtAction(nameof(Get), new { id = entity.UserId }, createdDto);
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
        public async Task<IActionResult> Update(int id, [FromBody] UserDetailDTO userDTO)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (auth.AuthUserId != id)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _manager.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            var updated = _mapper.Map<User>(userDTO);
            await _manager.UpdateAsync(existing, updated);
            return NoContent();
        }

        /// <summary>
        /// Update user's random seed
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <param name="model">The seed patch model</param>
        /// <returns>No content on success</returns>
        [HttpPatch("seed/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchSeed(int id, [FromBody] SeedPatch model)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (auth.AuthUserId != id)
                return Forbid();

            if (model == null || string.IsNullOrEmpty(model.Seed))
                return BadRequest();

            await _manager.PatchSeedAsync(id, model.Seed);
            return NoContent();
        }

        /// <summary>
        /// Update user's password
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <param name="model">The password patch model with current and new password</param>
        /// <returns>No content on success</returns>
        [HttpPatch("password/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PatchPassword(int id, [FromBody] PasswordPatch model)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated || !auth.AuthUserId.HasValue)
                return Unauthorized();
            if (auth.AuthUserId != id)
                return Forbid();

            if (model == null || string.IsNullOrEmpty(model.Hash) || string.IsNullOrEmpty(model.CurrentPassword))
                return BadRequest();

            var callingUser = await _manager.GetByIdAsync(auth.AuthUserId.Value);
            if (callingUser == null)
                return Unauthorized();

            // Placeholder password verification: compare raw values
            if (callingUser.HashPassword != model.CurrentPassword)
                return Unauthorized();

            await _manager.PatchPasswordAsync(id, model.Salt, model.Hash);
            return NoContent();
        }

        /// <summary>
        /// Soft delete a user account
        /// </summary>
        /// <param name="id">The ID of the user to delete</param>
        /// <param name="model">Password confirmation model</param>
        /// <returns>No content on success</returns>
        [HttpDelete("remove/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromBody] PasswordConfirm model)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated || !auth.AuthUserId.HasValue)
                return Unauthorized();
            if (!auth.IsAdmin && auth.AuthUserId != id)
                return Forbid();

            if (model == null || string.IsNullOrEmpty(model.Password))
                return BadRequest();

            User? callingUser = await _manager.GetByIdAsync(auth.AuthUserId.Value);
            if (callingUser == null)
                return Unauthorized();

            // Placeholder password verification
            if (callingUser.HashPassword != model.Password)
                return Unauthorized();

            var user = await _manager.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            await _manager.SoftDeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Request password reset for a user
        /// </summary>
        /// <param name="emailOrLogin">The email or login of the user</param>
        /// <returns>No content (always returns success for security)</returns>
        [AllowAnonymous]
        [HttpHead("resetpassword")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ResetPassword([FromQuery] string emailOrLogin)
        {
            if (string.IsNullOrEmpty(emailOrLogin))
                return BadRequest();

            // In real implementation: find user and send reset email
            return NoContent();
        }

        /// <summary>
        /// Verify user's phone number with code
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <param name="code">The verification code</param>
        /// <returns>No content on success</returns>
        [HttpHead("verifyphone")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyPhone([FromQuery] int id, [FromQuery] string code)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (auth.AuthUserId != id)
                return Forbid();

            var ok = await _manager.VerifyAsync(id, code);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// Verify user's email address with code
        /// </summary>
        /// <param name="id">The ID of the user</param>
        /// <param name="code">The verification code</param>
        /// <returns>No content on success</returns>
        [HttpHead("verifymail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> VerifyMail([FromQuery] int id, [FromQuery] string code)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (auth.AuthUserId != id)
                return Forbid();

            var ok = await _manager.VerifyAsync(id, code);
            return ok ? NoContent() : NotFound();
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
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (!auth.IsAdmin && auth.AuthUserId != id)
                return Forbid();

            var user = await _manager.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Authenticate user with login credentials
        /// </summary>
        /// <param name="model">Login credentials (login and password)</param>
        /// <returns>JWT token on successful authentication</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
                return BadRequest();

            var user = await _manager.GetByKeyAsync(model.Login);
            if (user == null) return Unauthorized();

            // TODO: Replace with proper password hashing and verification
            return Ok(new { Token = JwtService.GenerateJwtToken(user, _configuration) });
        }

        /// <summary>
        /// Set remember token for user session
        /// </summary>
        /// <param name="model">Remember token model</param>
        /// <returns>No content on success</returns>
        [AllowAnonymous]
        [HttpPost("remember")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Remember([FromBody] RememberModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.RememberToken))
                return BadRequest();

            // Placeholder: associate remember token with user
            return NoContent();
        }

        // Simple models used by endpoints
        public class SeedPatch { public string Seed { get; set; } = null!; }
        public class PasswordPatch { public string? Salt { get; set; } public string Hash { get; set; } = null!; public string CurrentPassword { get; set; } = null!; }
        public class LoginModel { public string Login { get; set; } = null!; public string Password { get; set; } = null!; }
        public class RememberModel { public string RememberToken { get; set; } = null!; }
        public class PasswordConfirm { public string Password { get; set; } = null!; }
    }
}
