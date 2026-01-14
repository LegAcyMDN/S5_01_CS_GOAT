using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Manages item upgrade/downgrade operations where users can combine items to potentially get better ones
    /// </summary>
    public interface IUpgradeRepository
    {
        /// <summary>
        /// Performs an upgrade operation where a user combines inventory items
        /// </summary>
        /// <param name="dto">Contains the items to upgrade, monetary value to add, and targeting options</param>
        /// <param name="userId">The ID of the user performing the upgrade</param>
        /// <returns>Details of the upgrade result including success rate, new item, and wallet changes</returns>
        Task<UpgradeOutputDTO> UpgradeAsync(UpgradeInputDTO dto, int userId);
    }
}
