using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages case opening transactions with provably fair randomization records
    /// </summary>
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
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            IEnumerable<RandomTransaction?> transactions = await manager.GetAllAsync();
            IEnumerable<RandomTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<RandomTransactionDTO>>(transactions);
            return Ok(new GetOptions<RandomTransactionDTO>(Request, transactionsDTO));
        }

        /// <summary>
        /// Get random transactions for the authenticated user
        /// </summary>
        /// <returns>List of RandomTransactionDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            IEnumerable<RandomTransaction> transactions = await authResult.GetByUser(manager, false);
            IEnumerable<RandomTransactionDTO> transactionsDTO = mapper.Map<IEnumerable<RandomTransactionDTO>>(transactions);
            return Ok(new GetOptions<RandomTransactionDTO>(Request, transactionsDTO));
        }

        /// <summary>
        /// Get RandomTransaction details by ID with full case and item information
        /// </summary>
        /// <param name="id">The ID of the transaction</param>
        /// <returns>RandomTransactionDetailDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            QueryOptions<RandomTransaction> queryOptions = new QueryOptions<RandomTransaction>()
                .Before(
                    rt => rt.Case,
                    rt => rt.InventoryItem.Wear.WearType,
                    rt => rt.InventoryItem.Wear.Skin.Rarity,
                    rt => rt.InventoryItem.Wear.Skin.Item.ItemType
                );
            RandomTransaction? result = await manager.GetByIdAsync(id, queryOptions);
            return result == null ? NotFound() : Ok(mapper.Map<RandomTransactionDetailDTO>(result));
        }

        /// <summary>
        /// Get live feed of recent case openings (public endpoint)
        /// </summary>
        /// <returns>List of recent LiveFeedDTO objects with item drops</returns>
        [HttpGet("livefeed")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> LiveFeed()
        {
            QueryOptions<RandomTransaction> queryOptions = new QueryOptions<RandomTransaction>()
                .Before(rt => rt.CaseId != null)
                .Before(
                    rt => rt.InventoryItem.Wear.WearType,
                    rt => rt.InventoryItem.Wear.Skin.Rarity,
                    rt => rt.InventoryItem.Wear.Skin.Item
                );

            IEnumerable<RandomTransaction> transactions = await manager.GetAllAsync(queryOptions);
            IEnumerable<LiveFeedDTO> liveFeedDTOs = mapper.Map<IEnumerable<LiveFeedDTO>>(transactions);
            return Ok(new GetOptions<LiveFeedDTO>(Request, liveFeedDTOs));
        }
    }
}