using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Provides operations for opening cases and managing case results
    /// </summary>
    public interface ICaseOpenningRepository
    {
        /// <summary>
        /// Opens one or more cases for a user and returns the results
        /// </summary>
        /// <param name="caseOpenningDTO">Details of the case(s) to open including case ID, quantity, and optional promo code</param>
        /// <param name="userId">The ID of the user opening the case</param>
        /// <returns>Details of the case opening result(s) including items obtained and wallet changes</returns>
        Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            int userId);

        /// <summary>
        /// Opens one or more cases for a user and returns the results
        /// </summary>
        /// <param name="caseOpenningDTO">Details of the case(s) to open including case ID, quantity, and optional promo code</param>
        /// <param name="user">The user opening the case</param>
        /// <returns>Details of the case opening result(s) including items obtained and wallet changes</returns>
        Task<MultipleCaseResultDTO> OpenCaseAsync(
            CaseOpenningDTO caseOpenningDTO,
            User user);
    }
}
