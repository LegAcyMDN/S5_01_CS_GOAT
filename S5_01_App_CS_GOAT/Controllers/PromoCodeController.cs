using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/PromoCode")]
    [ApiController]
    [SetThreadPrincipal]
    public class PromoCodeController(
        IMapper mapper,
        IPromoCodeRepository manager,
        IReadableRepository<Case, int> caseRepository,
        IConfiguration configuration
    ) : ControllerBase
    {
        /// <summary>
        /// Check if code is valid and retrieve promo code details
        /// </summary>
        /// <param name="code"></param>
        /// <returns>The promocode object if valid and usable by the user</returns>
        [HttpGet("check/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Check(string code, int? caseId)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            Case? targetCase = null;
            if (caseId != null)
            {
                targetCase = await caseRepository.GetByIdAsync(caseId.Value);
                if (targetCase == null)
                {
                    return NotFound();
                }
            }
            PromoCode? promoCode = await manager.Check(
                code,
                authResult.AuthUserId.Value,
                caseId
            );
            if (promoCode == null)
            {
                return NotFound();
            }

            CasePromoCodeDTO dto = mapper.Map<CasePromoCodeDTO>(promoCode);
            if (targetCase != null)
            {
                dto.BasePrice = targetCase.CasePrice;
                dto.FinalPrice = promoCode.Apply(targetCase.CasePrice);
            }
            return Ok(dto);
        }

        /// <summary>
        /// Get all promo codes with GetOptions support (admin only)
        /// </summary>
        /// <returns>GetOptions response with PromoCodeDTO list</returns>
        [HttpGet("all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll()
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            QueryOptions<PromoCode> queryOptions = new QueryOptions<PromoCode>()
                .Before(p => p.User, p => p.Case);
            IEnumerable<PromoCode> promoCodes = await manager.GetAllAsync(queryOptions);
            IEnumerable<PromoCodeDTO> promoCodeDtos = promoCodes
                .Select(pc => mapper.Map<PromoCodeDTO>(pc));

            return Ok(new GetOptions<PromoCodeDTO>(Request, promoCodeDtos));
        }

        /// <summary>
        /// Create a new promo code (admin only)
        /// </summary>
        /// <param name="promoCodeDto">The PromoCodeDTO object to create</param>
        /// <returns>The created PromoCode object</returns>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] PromoCodeDTO promoCodeDto)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Mapper DTO -> Entity
            PromoCode promoCode = mapper.Map<PromoCode>(promoCodeDto);

            PromoCode createdPromoCode = await manager.AddAsync(promoCode);

            // Mapper l'entité créée vers DTO pour la réponse
            PromoCodeDTO createdDto = mapper.Map<PromoCodeDTO>(createdPromoCode);

            return CreatedAtAction(nameof(GetAll), new { id = createdPromoCode.PromoCodeId }, createdDto);
        }

        /// <summary>
        /// Update an existing promo code (admin only)
        /// </summary>
        /// <param name="id">The ID of the promo code to update</param>
        /// <param name="promoCodeDto">The updated PromoCodeDTO object</param>
        /// <returns>No content on success</returns>
        [HttpPut("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] PromoCodeDTO promoCodeDto)
        {
            AuthResult authResult = JwtService.JwtAuth(configuration);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            PromoCode? existingPromoCode = await manager.GetByIdAsync(id);
            if (existingPromoCode == null)
            {
                return NotFound();
            }

            // Mapper les modifications du DTO vers l'entité existante
            PromoCode updatedPromoCode = mapper.Map<PromoCode>(promoCodeDto);

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
            {
                return Unauthorized();
            }

            if (!authResult.IsAdmin)
            {
                return Forbid();
            }

            PromoCode? promoCode = await manager.GetByIdAsync(id);
            if (promoCode == null)
            {
                return NotFound();
            }

            await manager.DeleteAsync(promoCode);
            return NoContent();
        }
    }
}