using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/limit")]
    [ApiController]
    public class LimitController(
       IMapper mapper,
       IDataRepository<Limit, int, (int, int)> manager,
       CSGOATDbContext context
       ) : ControllerBase
    {
        // PATCH api/limit/{userId}/{limitTypeId}
        [HttpPatch("{userId}/{limitTypeId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchLimit(int userId, int limitTypeId, [FromBody] LimitDTO limitDto)
        {
            if (limitDto == null)
                return BadRequest("LimitDTO cannot be null");

            Limit? existingLimit = await manager.GetByIdsAsync(userId, limitTypeId);
            
            if (existingLimit == null)
                return NotFound($"Limit not found for UserId: {userId} and LimitTypeId: {limitTypeId}");


            existingLimit.LimitAmount = limitDto.LimitAmount;

            if (!string.IsNullOrEmpty(limitDto.LimitTypeName) || !string.IsNullOrEmpty(limitDto.DurationName))
            {
                var newLimitType = context.LimitTypes.FirstOrDefault(lt => 
                    lt.LimitTypeName == limitDto.LimitTypeName && 
                    lt.DurationName == limitDto.DurationName);

                if (newLimitType != null && newLimitType.LimitTypeId != limitTypeId)
                {
                    // Si on change le type de limite, il faut supprimer l'ancienne et créer une nouvelle
                    // car LimitTypeId fait partie de la clé primaire
                    return BadRequest("Cannot change LimitType. Delete the old limit and create a new one.");
                }
            }

            // Mettre à jour l'entité
            await manager.PatchAsync(existingLimit, limitDto);

            return NoContent();
        }
    }
}

