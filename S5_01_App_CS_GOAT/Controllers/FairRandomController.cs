using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/FairRandom")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class FairRandomController(
        IMapper mapper,
        IDataRepository<FairRandom, int> manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get fair randoms for the authenticated user
        /// </summary>
        /// <returns>List of FairRandomDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<FairRandom> fairRandoms = await authResult.GetByUser(manager, false);

            IEnumerable<FairRandomDTO> userFairRandomsDTO = mapper.Map<IEnumerable<FairRandomDTO>>(fairRandoms);
            return Ok(userFairRandomsDTO);
        }
    }
}
