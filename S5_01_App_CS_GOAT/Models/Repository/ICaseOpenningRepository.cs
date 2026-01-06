using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface ICaseOpenningRepository
    {
        Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            int userId);

        Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            User user);
    }
}
