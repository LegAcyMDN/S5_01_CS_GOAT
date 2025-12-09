using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/PaymentMethod")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class PaymentMethodController(
        ITypeRepository<PaymentMethod> manager
    ) : ControllerBase
    {
        /// <summary>
        /// Get all payment methods
        /// </summary>
        /// <returns>List of all PaymentMethod objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<PaymentMethod> payments = await manager.GetAllAsync();
            return Ok(payments);
        }
    }
}

