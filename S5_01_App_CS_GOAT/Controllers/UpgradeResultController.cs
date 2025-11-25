using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UpgradeResultController : ControllerBase
    {
        private readonly UpgradeResultManager _manager;
        private readonly IConfiguration _configuration;

        public UpgradeResultController(UpgradeResultManager manager, IConfiguration configuration)
        {
            _manager = manager;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        public IActionResult GetUpgradeResult(int id)
        {
            var authResult = JwtService.JwtAuth(_configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            var result = _manager.GetUpgradeResult(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("create")]
        public IActionResult CreateUpgradeResult([FromQuery] UpgradeResultDTO dto)
        {
            var authResult = JwtService.JwtAuth(_configuration);
            if (!authResult.IsAuthorized)
            {
                return Unauthorized();
            }

            var createdResult = _manager.CreateUpgradeResult(dto);
            return Ok(createdResult);
        }
    }
}