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
        IFairRandomRepository manager,
        IConfiguration configuration) : ControllerBase
    {
        /// <summary>
        /// Get fair randoms for the authenticated user
        /// </summary>
        /// Only returns resolved FairRandoms
        /// <returns>List of FairRandomDTO objects for the user</returns>
        [HttpGet("byuser")]
        [Authorize]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();

            IEnumerable<FairRandom> fairRandoms = await authResult.GetByUser(manager, false, fn => fn.IsResolved);

            IEnumerable<FairRandomDTO> userFairRandomsDTO = mapper.Map<IEnumerable<FairRandomDTO>>(fairRandoms);
            return Ok(userFairRandomsDTO);
        }

        /// <summary>
        /// Get unresolved FairRandom ServerHash for user
        /// </summary>
        /// Can dynamically create a new FairRandom if none exists
        /// <returns>ServerHash string</returns>
        /// <response code="200">Returns the ServerHash string</response>
        [HttpGet("serverhash")]
        [Authorize]
        [AllowAnonymous]
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
