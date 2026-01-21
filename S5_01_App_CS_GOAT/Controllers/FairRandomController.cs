using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages provably fair random number sessions for verifiable gaming operations
    /// </summary>
    [Route("api/FairRandom")]
    [ApiController]
    [SetThreadPrincipal]
    public class FairRandomController(
        IMapper mapper,
        IFairRandomRepository manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get resolved fair random sessions for the authenticated user (for verification)
        /// </summary>
        /// Only returns resolved FairRandoms
        /// <returns>List of FairRandomDTO objects for the user</returns>
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

            QueryOptions<FairRandom> queryOptions = new QueryOptions<FairRandom>()
                .Before(fr => fr.UserId == null)
                .Before(fr => fr.RandomTransaction)
                .Before("UpgradeResult.RandomTransaction");
            IEnumerable<FairRandom> fairRandoms = await authResult.GetByUser(manager, false, queryOptions);

            IEnumerable<FairRandomDTO> userFairRandomsDTO = mapper.Map<IEnumerable<FairRandomDTO>>(fairRandoms);
            return Ok(new GetOptions<FairRandomDTO>(Request, userFairRandomsDTO));
        }

        /// <summary>
        /// Get unresolved FairRandom server hash for the current session
        /// </summary>
        /// Can dynamically create a new FairRandom session if none exists
        /// <returns>ServerHash string for client-side verification</returns>
        [HttpGet("serverhash")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetServerHash()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            FairRandom next = await manager.Init(authResult.AuthUserId!.Value, true);
            return Ok(next.ServerHash);
        }
    }
}
