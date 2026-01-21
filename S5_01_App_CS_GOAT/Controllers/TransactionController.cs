using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages transaction cancellation and refunds (admin operations)
    /// </summary>
    [Route("api/Transaction")]
    [ApiController]
    [SetThreadPrincipal]
    public class TransactionController(
        IConfiguration configuration
    ) : ControllerBase
    {

        /// <summary>
        /// Cancel/remove a transaction and process refund (admin only)
        /// </summary>
        /// <param name="id">The ID of the transaction to cancel</param>
        /// <returns>No content on success</returns>
        [HttpDelete("remove/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            return !authResult.IsAdmin ? (IActionResult)Forbid() : throw new NotImplementedException();
        }
    }
}
