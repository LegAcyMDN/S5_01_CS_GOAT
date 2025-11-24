using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{

    [Route("api/transaction")]
    [ApiController]
    public class MoneyTransactionController(
               // IMapper mapper,
               IDataRepository<MoneyTransaction, int, string> manager,
               CSGOATDbContext context
               ) : ControllerBase
    {


        [HttpGet("ByUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MoneyTransaction>>> GetByUser()
        {
            AuthResult authResult = JwtService.Authorized(null);
            int? id = authResult.AuthUserId;

            IEnumerable<MoneyTransaction?> payment = await manager.GetAllAsync();
            IEnumerable<MoneyTransaction?> userPayments = payment.Where(p => p.DependantUserId == id);
            if (payment == null || !payment.Any())
                return NotFound();
            return Ok(userPayments);
        }


        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MoneyTransaction>>> GetAll()
        {
            IEnumerable<MoneyTransaction?> payment = await manager.GetAllAsync();
            if (payment == null || !payment.Any())
                return NotFound();
            return Ok(payment);
        }

    }
}

