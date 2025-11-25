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
    [Route("api/transactions")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly TransactionManager _transactionManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public TransactionController(IDataRepository<Transaction, int, string> manager, IMapper mapper, IConfiguration configuration)
        {
            _transactionManager = (TransactionManager)manager;
            _mapper = mapper;
            _configuration = configuration;
        }

        [HttpDelete("remove/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
                return Unauthorized();
            if (!auth.IsAdmin)
                return Forbid();

            Transaction? transaction = await _transactionManager.GetByIdAsync(id);
            if (transaction == null)
                return NotFound();

            await _transactionManager.SetCancelledOnAsync(id);
            return NoContent();
        }
    }
}
