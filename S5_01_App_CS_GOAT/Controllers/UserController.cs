using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(/*IMapper mapper,*/ IDataRepository<User, int, string> manager, CSGOATDbContext context) : ControllerBase
{
    [HttpGet("details/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        User? user = await manager.GetByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpDelete("remove/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        User? user = await manager.GetByIdAsync(id);
        if (user == null)
            return NotFound();
        await manager.DeleteAsync(user);
        return NoContent();
    }

    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<User>>> GetAll()
    {
        IEnumerable<User?> user = await manager.GetAllAsync();
        if (user == null || !user.Any())
            return NotFound();
        return Ok(user);
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await manager.AddAsync(user);

        return CreatedAtAction("Get", new { id = user.UserId }, user);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        User? userToUpdate = await manager.GetByIdAsync(id);

        if (userToUpdate == null)
        {
            return NotFound();
        }
        await manager.UpdateAsync(userToUpdate, user);

        return NoContent();
    }
}
