using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    public interface ICaseOpenningService
    {
        Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            int userId);

        Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            User user);
    }

    public interface ISellingService
    {
        Task<int> SellAsync(int invItemId);

        Task<int> SellAsync(InventoryItem invItem);
    }
}
