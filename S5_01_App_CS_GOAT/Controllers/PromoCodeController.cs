using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.DataManager;

namespace S5_01_App_CS_GOAT.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PromoCodeController : ControllerBase
{
    private readonly PromoCodeManager _manager;

    public PromoCodeController(PromoCodeManager manager)
    {
        _manager = manager;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PromoCode>>> GetAll([FromQuery] FilterOptions? filters, [FromQuery] SortOptions? sorts)
    {
        var promoCodes = await _manager.GetAllAsync(filters, sorts);
        return Ok(promoCodes);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PromoCode>> Create([FromBody] PromoCode promoCode)
    {
        var createdPromoCode = await _manager.AddAsync(promoCode);
        return CreatedAtAction(nameof(GetAll), new { id = createdPromoCode.PromoCodeId }, createdPromoCode);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] PromoCode promoCode)
    {
        var existingPromoCode = await _manager.GetByIdAsync(id);
        if (existingPromoCode == null) return NotFound();

        await _manager.UpdateAsync(existingPromoCode, promoCode);
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var promoCode = await _manager.GetByIdAsync(id);
        if (promoCode == null) return NotFound();

        await _manager.DeleteAsync(promoCode);
        return NoContent();
    }
}