using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/FairRandom")]
    [ApiController]
    [SetThreadPrincipal]
    public class FairRandomController(
        IMapper mapper,
        IFairRandomRepository manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get fair randoms for the authenticated user
        /// </summary>
        /// Only returns resolved FairRandoms
        /// <returns>List of FairRandomDTO objects for the user</returns>
        [HttpGet("byuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<FairRandom> fairRandoms = await authResult.GetByUser(manager, false, fr => fr.UserId == null, "RandomTransaction", "UpgradeResult.RandomTransaction");

            IEnumerable<FairRandomDTO> userFairRandomsDTO = mapper.Map<IEnumerable<FairRandomDTO>>(fairRandoms);
            return Ok(new GetOptions<FairRandomDTO>(Request, userFairRandomsDTO));
        }

        /// <summary>
        /// Get unresolved FairRandom ServerHash for user
        /// </summary>
        /// Can dynamically create a new FairRandom if none exists
        /// <returns>ServerHash string</returns>
        /// <response code="200">Returns the ServerHash string</response>
        [HttpGet("serverhash")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServerHash()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            FairRandom next = await manager.Init(authResult.AuthUserId.Value, true);
            return Ok(next.ServerHash);
        }
    }
}
