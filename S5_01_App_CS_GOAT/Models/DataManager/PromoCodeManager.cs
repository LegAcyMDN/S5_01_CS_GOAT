using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    public class PromoCodeManager : CrudRepository<PromoCode, int>, IPromoCodeRepository
    {
        protected readonly CSGOATDbContext _context;

        public PromoCodeManager(CSGOATDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task Refresh(PromoCode promoCode)
        {
            if (!promoCode.IsDueForRefresh()) return;
            promoCode.ValidityStart += promoCode.RefreshDelay.Value;
            promoCode.ExpiryDate += promoCode.RefreshDelay.Value;
            if (promoCode.RemainingUses == 0) promoCode.RemainingUses = 1;
            await this.UpdateAsync(promoCode);
            return;
        }

        public async Task<bool> CheckValidity(PromoCode promoCode)
        {
            await this.Refresh(promoCode);
            if (promoCode.IsValid()) return true;
            if (promoCode.IsDueForDelete())
                await this.DeleteAsync(promoCode);
            return false;
        }

        public async Task<PromoCode?> Check(string code, int userId, int? caseId = null)
        {
            IEnumerable<PromoCode> promoCodes = await GetAllAsync(
                pc => pc.Code == code &&
                (pc.UserId == null || pc.UserId == userId) &&
                (pc.CaseId == null || pc.CaseId == caseId)
                );
            PromoCode? promoCode = promoCodes.FirstOrDefault();
            if (promoCode == null) return null;
            bool isValid = await CheckValidity(promoCode);
            if (!isValid) return null;
            return promoCode;
        }

        public async Task Consume(PromoCode promoCode)
        {
            if (promoCode.RemainingUses != null && promoCode.RemainingUses > 0)
            {
                promoCode.RemainingUses -= 1;
                await this.UpdateAsync(promoCode);
            }
            await CheckValidity(promoCode);
        }
    }
}
