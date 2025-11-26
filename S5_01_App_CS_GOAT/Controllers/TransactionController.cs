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
    [Route("api/Transaction")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class TransactionController(
        IDataRepository<Transaction, int, string> manager
    ) : ControllerBase
    {
        private readonly TransactionManager _transactionManager = (TransactionManager)manager;

        /// <summary>
        /// Cancel/remove a transaction (admin only)
        /// </summary>
        /// <param name="id">The ID of the transaction to cancel</param>
        /// <returns>No content on success</returns>
        [HttpDelete("remove/{id}")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            Transaction? transaction = await _transactionManager.GetByIdAsync(id);
            if (transaction == null) return NotFound();

            await _transactionManager.SetCancelledOnAsync(id);
            return NoContent();
        }
    }
}
