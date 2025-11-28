using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{

    [Route("api/itemTransaction")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class ItemTransactionController(
       IMapper mapper,
       IDataRepository<ItemTransaction, int, string> manager,
       IConfiguration configuration
       ) : ControllerBase
    {

        /// <summary>
        /// Get all promo codes (admin only)
        /// </summary>
        /// <param name="filters">Optional filter parameters</param>
        /// <param name="sorts">Optional sort parameters</param>
        /// <returns>List of all PromoCode objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if(!authResult.IsAdmin)
                return Forbid();

            IEnumerable<ItemTransaction> promoCodes = await manager.GetAllAsync();
             IEnumerable<ItemTransactionDTO> promoCodesDTO = mapper.Map<IEnumerable<ItemTransactionDTO>>(promoCodes);
            return Ok(promoCodesDTO);
        }

        /// <summary>
        /// Get case details by ID
        /// </summary>
        /// <param name="id">The ID of the case</param>
        /// <returns>CaseDetailDTO object</returns>
        [HttpGet("details/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            ItemTransaction? itemTransaction = await manager.GetByIdAsync(id);
            if (itemTransaction == null) return NotFound();
            if (authResult.AuthUserId != itemTransaction.DependantUserId && !authResult.IsAdmin)
                return Forbid();
            ItemTransactionDetailDTO itemTransactionDetail = mapper.Map<ItemTransactionDetailDTO>(itemTransaction);

            return Ok(itemTransactionDetail);
        }

        /// <summary>
        /// Get inventory items for the authenticated user
        /// </summary>
        /// <returns>List of InventoryItemDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<ItemTransaction> itemTransactions = await authResult.GetByUser(manager, false);
            IEnumerable<ItemTransactionDTO> itemTransactionDTO = mapper.Map<IEnumerable<ItemTransactionDTO>>(itemTransactions);
            return Ok(itemTransactionDTO);
        }
    }
}
