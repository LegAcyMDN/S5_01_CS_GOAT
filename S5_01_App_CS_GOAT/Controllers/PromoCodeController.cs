using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/PromoCode")]
    [ApiController]
    [Authorize]
    [AllowAnonymous]
    public class PromoCodeController(
        IDataRepository<PromoCode, int> manager,
        IConfiguration configuration
    ) : ControllerBase
    {

        /// <summary>
        /// Get all promo codes (admin only)
        /// </summary>
        /// <returns>List of all PromoCode objects</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();
            IEnumerable<PromoCode> promoCodes = await manager.GetAllAsync();
            return Ok(promoCodes);
        }

        /// <summary>
        /// Create a new promo code (admin only)
        /// </summary>
        /// <param name="promoCode">The PromoCode object to create</param>
        /// <returns>The created PromoCode object</returns>
        [HttpPost("create")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PromoCode promoCode)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();
            if (!ModelState.IsValid) return BadRequest(ModelState);

            PromoCode createdPromoCode = await manager.AddAsync(promoCode);
            return CreatedAtAction(nameof(GetAll), new { id = createdPromoCode.PromoCodeId }, createdPromoCode);
        }

        /// <summary>
        /// Update an existing promo code (admin only)
        /// </summary>
        /// <param name="id">The ID of the promo code to update</param>
        /// <param name="updatedPromoCode">The updated PromoCode object</param>
        /// <returns>No content on success</returns>
        [HttpPut("update/{id}")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] PromoCode updatedPromoCode)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            PromoCode? existingPromoCode = await manager.GetByIdAsync(id);
            if (existingPromoCode == null) return NotFound();

            await manager.UpdateAsync(existingPromoCode, updatedPromoCode);
            return NoContent();
        }

        /// <summary>
        /// Delete a promo code (admin only)
        /// </summary>
        /// <param name="id">The ID of the promo code to delete</param>
        /// <returns>No content on success</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
                return Unauthorized();
            if (!authResult.IsAdmin)
                return Forbid();
            var promoCode = await manager.GetByIdAsync(id);
            if (promoCode == null) 
                return NotFound();

            await manager.DeleteAsync(promoCode);
            return NoContent();
        }
    }
}