using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers;

[Route("api/payments")]
[ApiController]
public class PaymentMethodController(
           // IMapper mapper,
           IDataRepository<PaymentMethod, int, string> manager,
           CSGOATDbContext context
           ) : ControllerBase
{
    [HttpGet("details/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int id)
    {
        PaymentMethod? payment = await manager.GetByIdAsync(id);
        return payment == null ? NotFound() : Ok(payment);
    }


    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PaymentMethod>>> GetAll()
    {
        IEnumerable<PaymentMethod?> payment = await manager.GetAllAsync();
        if (payment == null || !payment.Any())
            return NotFound();
        return Ok(payment);
    }
}

