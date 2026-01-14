using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

/// <summary>
/// Manages promotional codes and their validation, application, and consumption
/// </summary>
public interface IPromoCodeRepository : IDataRepository<PromoCode, int>
{
    /// <summary>
    /// Checks if a promotional code is still valid and refreshes it if needed
    /// </summary>
    /// <param name="promoCode">The promotional code to validate</param>
    /// <returns>True if the code is valid; false if expired or marked for deletion</returns>
    Task<bool> CheckValidity(PromoCode promoCode);

    /// <summary>
    /// Refreshes a promotional code by extending its validity window if due for refresh
    /// </summary>
    /// <param name="promoCode">The promotional code to refresh</param>
    Task Refresh(PromoCode promoCode);

    /// <summary>
    /// Retrieves and validates a promotional code for a specific user and optional case
    /// </summary>
    /// <param name="code">The promotional code string</param>
    /// <param name="userId">The ID of the user applying the code</param>
    /// <param name="caseId">Optional case ID the code must be valid for (null if code applies to any case)</param>
    /// <returns>The promotional code if valid; otherwise null</returns>
    Task<PromoCode?> Check(string code, int userId, int? caseId = null);

    /// <summary>
    /// Consumes one use of a promotional code by decrementing remaining uses
    /// </summary>
    /// <param name="promoCode">The promotional code to consume</param>
    Task Consume(PromoCode promoCode);
}