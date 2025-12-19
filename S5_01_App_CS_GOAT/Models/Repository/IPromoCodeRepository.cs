using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Models.Repository;

public interface IPromoCodeRepository : IDataRepository<PromoCode, int>
{
    Task<bool> CheckValidity(PromoCode promoCode);

    Task Refresh(PromoCode promoCode);

    Task<PromoCode?> Check(string code, int userId, int? caseId = null);

    Task Consume(PromoCode promoCode);
}