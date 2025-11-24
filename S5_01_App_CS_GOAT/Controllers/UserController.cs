using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers;

[Route("api/users")]
[ApiController]
[Authorize] // require JWT by default; allow anonymous on specific endpoints below
public class UserController : ControllerBase
{
    private readonly UserManager _manager;
    private readonly IMapper _mapper;

    public UserController(UserManager manager, IMapper mapper)
    {
        _manager = manager;
        _mapper = mapper;
    }

    // Helper to use JwtService.Authorized
    private class OwnerDependant : IUserDependant { public int? DependantUserId { get; set; } }

    // GET api/users/all  (Admin)
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        // admin only
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = null });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAdmin)
            return Forbid();

        var users = await _manager.GetAllForAdminAsync();
        if (users == null || !users.Any())
            return NotFound();

        var dtos = _mapper.Map<IEnumerable<UserDetailDTO>>(users);
        return Ok(dtos);
    }

    // GET api/users/details/{id} (JWT)
    [HttpGet("details/{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        var user = await _manager.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        var dto = _mapper.Map<UserDetailDTO>(user);
        return Ok(dto);
    }

    // POST api/users/create (public)
    [AllowAnonymous]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] UserDetailDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = _mapper.Map<User>(dto);
        await _manager.AddAsync(entity);
        var createdDto = _mapper.Map<UserDetailDTO>(entity);
        return CreatedAtAction(nameof(Get), new { id = entity.UserId }, createdDto);
    }

    // PATCH api/users/update/{id} (JWT)
    [HttpPatch("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserDetailDTO dto)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var existing = await _manager.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        var updated = _mapper.Map<User>(dto);
        await _manager.UpdateAsync(existing, updated);
        return NoContent();
    }

    // PATCH api/users/seed/{id} (JWT)
    [HttpPatch("seed/{id}")]
    public async Task<IActionResult> PatchSeed(int id, [FromBody] SeedPatch model)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        if (model == null || string.IsNullOrEmpty(model.Seed))
            return BadRequest();

        await _manager.PatchSeedAsync(id, model.Seed);
        return NoContent();
    }

    // PATCH api/users/password/{id} (JWT + password confirmation)
    [HttpPatch("password/{id}")]
    public async Task<IActionResult> PatchPassword(int id, [FromBody] PasswordPatch model)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        if (model == null || string.IsNullOrEmpty(model.Hash) || string.IsNullOrEmpty(model.CurrentPassword))
            return BadRequest();

        // verify calling user's password
        if (auth.AuthUserId == null)
            return Unauthorized();

        var callingUser = await _manager.GetByIdAsync(auth.AuthUserId.Value);
        if (callingUser == null)
            return Unauthorized();

        // Placeholder password verification: compare raw values
        if (callingUser.HashPassword != model.CurrentPassword)
            return Unauthorized();

        await _manager.PatchPasswordAsync(id, model.Salt, model.Hash);
        return NoContent();
    }

    // DELETE api/users/remove/{id}  (soft-delete) (JWT + password)
    [HttpDelete("remove/{id}")]
    public async Task<IActionResult> Delete(int id, [FromBody] PasswordConfirm model)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        if (model == null || string.IsNullOrEmpty(model.Password))
            return BadRequest();

        if (auth.AuthUserId == null)
            return Unauthorized();

        var callingUser = await _manager.GetByIdAsync(auth.AuthUserId.Value);
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

    // HEAD api/users/resetpassword?emailOrLogin=... (public)
    [AllowAnonymous]
    [HttpHead("resetpassword")]
    public IActionResult ResetPassword([FromQuery] string emailOrLogin)
    {
        if (string.IsNullOrEmpty(emailOrLogin))
            return BadRequest();

        // In real implementation: find user and send reset email
        return NoContent();
    }

    // HEAD api/users/verifyphone?id=1&code=abc (JWT)
    [HttpHead("verifyphone")]
    public async Task<IActionResult> VerifyPhone([FromQuery] int id, [FromQuery] string code)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        var ok = await _manager.VerifyAsync(id, code);
        return ok ? NoContent() : NotFound();
    }

    // HEAD api/users/verifymail?id=1&code=abc (JWT)
    [HttpHead("verifymail")]
    public async Task<IActionResult> VerifyMail([FromQuery] int id, [FromQuery] string code)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        var ok = await _manager.VerifyAsync(id, code);
        return ok ? NoContent() : NotFound();
    }

    // HEAD api/users/exportdata?id=1 (JWT)
    [HttpHead("exportdata")]
    public async Task<IActionResult> ExportData([FromQuery] int id)
    {
        var auth = JwtService.Authorized(new OwnerDependant { DependantUserId = id });
        if (!auth.IsAuthenticated)
            return Unauthorized();
        if (!auth.IsAuthorized)
            return Forbid();

        var user = await _manager.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return NoContent();
    }

    // POST api/users/login (public)
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Login) || string.IsNullOrEmpty(model.Password))
            return BadRequest();

        var user = await _manager.GetByKeyAsync(model.Login);
        if (user == null)
            return Unauthorized();

        // NOTE: real password verification omitted. Return placeholder token.
        return Ok(new { Token = JwtService.GenerateJwtToken(user) });
    }

    // POST api/users/remember (public)
    [AllowAnonymous]
    [HttpPost("remember")]
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
