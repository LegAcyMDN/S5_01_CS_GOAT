using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [ApiController]
    [Route("api/promocode")]
    public class PromoCodeController : ControllerBase
    {
        private readonly PromoCodeManager _manager;
        private readonly IConfiguration _configuration;

        public PromoCodeController(IDataRepository<PromoCode, int, string> manager, IConfiguration configuration)
        {
            _manager = (PromoCodeManager)manager;
            _configuration = configuration;
        }

        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAdmin) return Unauthorized();

            var promoCodes = await _manager.GetAllAsync(filters, sorts);
            if (promoCodes == null || !promoCodes.Any()) return NotFound();

            return Ok(promoCodes);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] PromoCode promoCode)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAdmin) return Unauthorized();

            var createdPromoCode = await _manager.AddAsync(promoCode);
            return CreatedAtAction(nameof(GetAll), new { id = createdPromoCode.PromoCodeId }, createdPromoCode);
        }

        [HttpPut("put/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(int id, [FromBody] PromoCode updatedPromoCode)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAdmin) return Unauthorized();

            var existingPromoCode = await _manager.GetByIdAsync(id);
            if (existingPromoCode == null) return NotFound();

            await _manager.UpdateAsync(existingPromoCode, updatedPromoCode);
            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAdmin) return Unauthorized();

            var promoCode = await _manager.GetByIdAsync(id);
            if (promoCode == null) return NotFound();

            await _manager.DeleteAsync(promoCode);
            return NoContent();
        }
    }
}