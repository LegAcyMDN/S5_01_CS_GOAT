using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Mapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/randomtransaction")]
    [ApiController]
    public class RandomTransactionController(
        IMapper mapper,
       IRandomTransactiony<ItemTransaction> manager, IConfiguration configuration,
       IRandomTransactiony<RandomTransaction> randomanager
    ) : ControllerBase
    {

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ItemTransactionDTO>>> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            if (!authResult.IsAdmin)
                return Forbid();
            IEnumerable<ItemTransaction?> transactions = await manager.GetAllAsync();
            if (transactions == null || !transactions.Any())
                return NotFound();
            IEnumerable<ItemTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<ItemTransactionDTO>>(transactions);
            return Ok(transactionsDTO);
        }


        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ItemTransactionDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<ItemTransaction> transactions = await authResult.GetByUser(manager, false);
            if (!transactions.Any()) return NotFound();

            IEnumerable<ItemTransactionDTO> limitsDTO = mapper.Map<IEnumerable<ItemTransactionDTO>>(transactions);
            return Ok(limitsDTO);
        }

        /// <summary>
        /// Get RandomTransaction details by ID
        /// </summary>
        /// <param name="id">The ID of the transaction</param>
        /// <returns>ItemTransactionDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            ItemTransaction? result = await manager.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(mapper.Map<ItemTransactionDTO>(result));
        }

        [HttpGet("liveFeed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RandomTransaction>>> LiveFeed(int count)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<RandomTransaction?> transactions = await randomanager.GetRandomTransactionsAsync(count);

            if (transactions == null || !transactions.Any())
                return NotFound();
            return Ok(transactions);
        }

        // TODO: Add OpenCase  endpoint
    }
}