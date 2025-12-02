using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/RandomTransaction")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class RandomTransactionController(
        IMapper mapper,
        IDataRepository<ItemTransaction, int> manager,
        IConfiguration configuration
    ) : ControllerBase
    {
        /// <summary>
        /// Get all random transactions (admin only)
        /// </summary>
        /// <returns>List of all ItemTransactionDTO objects</returns>
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
            IEnumerable<ItemTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<ItemTransactionDTO>>(transactions);
            return Ok(transactionsDTO);
        }

        /// <summary>
        /// Get random transactions for the authenticated user
        /// </summary>
        /// <returns>List of ItemTransactionDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ItemTransactionDTO>>> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<ItemTransaction> transactions = await authResult.GetByUser(manager, false);
            IEnumerable<ItemTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<ItemTransactionDTO>>(transactions);
            return Ok(transactionsDTO);
        }

        /// <summary>
        /// Get RandomTransaction details by ID
        /// </summary>
        /// <param name="id">The ID of the transaction</param>
        /// <returns>ItemTransactionDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ItemTransactionDetailDTO>> Get(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            ItemTransaction? result = await manager.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(mapper.Map<ItemTransactionDetailDTO>(result));
        }

        /// <summary>
        /// Get live feed of random transactions
        /// </summary>
        /// <param name="count">Number of transactions to retrieve</param>
        /// <returns>List of RandomTransaction objects</returns>
        [HttpGet("livefeed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RandomTransaction>>> LiveFeed(int count)
        {

            throw new NotImplementedException();
        }

        // TODO: Add OpenCase endpoint
    }
}