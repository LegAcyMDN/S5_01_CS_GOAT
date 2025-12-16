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
    }
}
