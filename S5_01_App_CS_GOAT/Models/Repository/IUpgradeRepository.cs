using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface IUpgradeRepository
    {
        Task<UpgradeOutputDTO> UpgradeAsync(UpgradeInputDTO dto, int userId);
    }
}
