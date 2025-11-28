using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.DataManager;
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
        IDataRepository<PromoCode, int, string> manager
    ) : ControllerBase
    {
        private readonly PromoCodeManager _manager = (PromoCodeManager)manager;

        /// <summary>
        /// Get all promo codes (admin only)
        /// </summary>
        /// <param name="filters">Optional filter parameters</param>
        /// <param name="sorts">Optional sort parameters</param>
        /// <returns>List of all PromoCode objects</returns>
        [HttpGet("all")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
        {
            IEnumerable<PromoCode> promoCodes = await _manager.GetAllAsync(filters, sorts);
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
            if (!ModelState.IsValid) return BadRequest(ModelState);

            PromoCode createdPromoCode = await _manager.AddAsync(promoCode);
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
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            PromoCode? existingPromoCode = await _manager.GetByIdAsync(id);
            if (existingPromoCode == null) return NotFound();

            await _manager.UpdateAsync(existingPromoCode, updatedPromoCode);
            return NoContent();
        }

        /// <summary>
        /// Delete a promo code (admin only)
        /// </summary>
        /// <param name="id">The ID of the promo code to delete</param>
        /// <returns>No content on success</returns>
        [HttpDelete("delete/{id}")]
        [Admin]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var promoCode = await _manager.GetByIdAsync(id);
            if (promoCode == null) 
                return NotFound();

            await _manager.DeleteAsync(promoCode);
            return NoContent();
        }
    }
}