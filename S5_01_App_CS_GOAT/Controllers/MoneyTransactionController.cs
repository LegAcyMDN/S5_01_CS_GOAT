using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/MoneyTransaction")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    [SetThreadPrincipal]
    public class MoneyTransactionController(
        IMapper mapper,
        IDataRepository<MoneyTransaction, int> manager,
        IConfiguration configuration
        ) : ControllerBase
    {
        /// <summary>
        /// Get money transactions for the authenticated user
        /// </summary>
        /// <returns>List of MoneyTransactionDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<MoneyTransaction> transactions = await authResult.GetByUser(manager, false, null, "PaymentMethod");
            IEnumerable<MoneyTransactionDTO> transactionsDto = mapper.Map<IEnumerable<MoneyTransactionDTO>>(transactions);
            return Ok(transactionsDto);
        }

        /// <summary>
        /// Get all money transactions (admin only)
        /// </summary>
        /// <returns>List of all MoneyTransactionDTO objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();
            IEnumerable<MoneyTransaction> transactions = await manager.GetAllAsync(null, "PaymentMethod");
            IEnumerable<MoneyTransactionDTO> transactionsDto = mapper.Map<IEnumerable<MoneyTransactionDTO>>(transactions);
            return Ok(transactionsDto);
        }
    }
}

