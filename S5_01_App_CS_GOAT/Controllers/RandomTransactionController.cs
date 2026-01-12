using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.Mapper;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/RandomTransaction")]
    [ApiController]
    [SetThreadPrincipal]
    public class RandomTransactionController(
        IMapper mapper,
        IDataRepository<RandomTransaction, int> manager,
        IConfiguration configuration
    ) : ControllerBase
    {
        /// <summary>
        /// Get all random transactions (admin only)
        /// </summary>
        /// <returns>List of all RandomTransactionDTO objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();

            IEnumerable<RandomTransaction?> transactions = await manager.GetAllAsyncOld();
            IEnumerable<RandomTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<RandomTransactionDTO>>(transactions);
            return Ok(new GetOptions<RandomTransactionDTO>(Request, transactionsDTO));
        }

        /// <summary>
        /// Get random transactions for the authenticated user
        /// </summary>
        /// <returns>List of RandomTransactionDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<RandomTransaction> transactions = await authResult.GetByUser(manager, false);
            IEnumerable<RandomTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<RandomTransactionDTO>>(transactions);
            return Ok(new GetOptions<RandomTransactionDTO>(Request, transactionsDTO));
        }

        /// <summary>
        /// Get RandomTransaction details by ID
        /// </summary>
        /// <param name="id">The ID of the transaction</param>
        /// <returns>RandomTransactionDetailDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            RandomTransaction? result = await manager.GetByIdAsyncOld(id,
                "Case",
                "InventoryItem.Wear.WearType",
                "InventoryItem.Wear.Skin.Rarity",
                "InventoryItem.Wear.Skin.Item.ItemType");
            if (result == null) return NotFound();
            return Ok(mapper.Map<RandomTransactionDetailDTO>(result));
        }

        /// <summary>
        /// Get live feed of random transactions from cases
        /// </summary>
        /// <param name="count">Number of transactions to retrieve (default: 20)</param>
        /// <returns>List of RandomTransactionLiveFeedDTO objects</returns>
        [HttpGet("livefeed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LiveFeed(int count)
        {
            var queryOptions = new QueryOptions<RandomTransaction>()
                .Before(rt => rt.CaseId != null)
                .Before(
                    rt => rt.InventoryItem.Wear.WearType,
                    rt => rt.InventoryItem.Wear.Skin.Rarity,
                    rt => rt.InventoryItem.Wear.Skin.Item
                );

            var transactions = await manager.GetAllAsyncNew(queryOptions);

            var recentTransactions = transactions
                .OrderByDescending(t => t.TransactionDate)
                .Take(count)
                .ToList();

            var liveFeedDTOs = mapper.Map<IEnumerable<RandomTransactionLiveFeedDTO>>(recentTransactions);

            return Ok(liveFeedDTOs);
        }
    }
}