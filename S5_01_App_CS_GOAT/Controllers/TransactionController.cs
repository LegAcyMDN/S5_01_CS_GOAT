namespace S5_01_App_CS_GOAT.Controllers
{

    using AutoMapper;
    using global::AutoMapper;
    using global::S5_01_App_CS_GOAT.Models.EntityFramework;
    using global::S5_01_App_CS_GOAT.Models.Repository;
    using Microsoft.AspNetCore.Mvc;
    using System;

    namespace S5_01_App_CS_GOAT.Controllers
    {

        [Route("api/Transaction")]
        [ApiController]
        public class TransactionController(
            IMapper mapper,
            IDataRepository<Transaction, int, string> manager,
            CSGOATDbContext context
            ) : ControllerBase
        {

            [HttpDelete("remove/{id}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(StatusCodes.Status404NotFound)]
            public async Task<IActionResult> Delete(int id)
            {
                Transaction? transaction = await manager.GetByIdAsync(id);
                if (transaction == null)
                    return NotFound();
                await manager.DeleteAsync(transaction);
                return NoContent();
            }
        }
    }
}
