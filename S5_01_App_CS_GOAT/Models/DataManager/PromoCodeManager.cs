using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Manages promotional code validation, refreshing, and consumption
    /// </summary>
    public class PromoCodeManager : CrudRepository<PromoCode, int>, IPromoCodeRepository
    {
        protected new readonly CSGOATDbContext _context;

        public PromoCodeManager(CSGOATDbContext context) : base(context)
        {
            _context = context;
        }

        /// <summary>
        /// Refreshes an expired promo code by extending its validity period
        /// </summary>
        /// <param name="promoCode">The promo code to refresh</param>
        /// <remarks>
        /// If the promo code is due for refresh (expired and refresh delay passed):
        /// - Extends ValidityStart by RefreshDelay
        /// - Extends ExpiryDate by RefreshDelay
        /// - Resets RemainingUses to 1 if it was 0
        /// </remarks>
        public async Task Refresh(PromoCode promoCode)
        {
            if (!promoCode.IsDueForRefresh())
            {
                return;
            }

            promoCode.ValidityStart += promoCode.RefreshDelay!.Value;
            promoCode.ExpiryDate += promoCode.RefreshDelay!.Value;
            if (promoCode.RemainingUses == 0)
            {
                promoCode.RemainingUses = 1;
            }

            await UpdateAsync(promoCode);
            return;
        }

        /// <summary>
        /// Checks and updates the validity status of a promo code
        /// </summary>
        /// <param name="promoCode">The promo code to validate</param>
        /// <returns>True if the code is valid after checks; false if deleted or invalid</returns>
        /// <remarks>
        /// This method:
        /// 1. Attempts to refresh the code if eligible
        /// 2. Returns true if the code is still valid
        /// 3. Deletes the code if marked for deletion
        /// </remarks>
        public async Task<bool> CheckValidity(PromoCode promoCode)
        {
            await Refresh(promoCode);
            if (promoCode.IsValid())
            {
                return true;
            }

            if (promoCode.IsDueForDelete())
            {
                await DeleteAsync(promoCode);
            }

            return false;
        }

        /// <summary>
        /// Retrieves and validates a promo code by code string
        /// </summary>
        /// <param name="code">The promo code string</param>
        /// <param name="userId">The user requesting the code</param>
        /// <param name="caseId">Optional case ID to filter codes (if null, accepts codes for any case)</param>
        /// <returns>The promo code if valid; null if not found or invalid</returns>
        /// <remarks>
        /// This method finds a code that matches the string and is applicable to the user/case.
        /// It then validates the code and returns null if validation fails.
        /// </remarks>
        public async Task<PromoCode?> Check(string code, int userId, int? caseId = null)
        {
            QueryOptions<PromoCode> options = new QueryOptions<PromoCode>()
                .Before(pc => pc.Code == code &&
                    (pc.UserId == null || pc.UserId == userId) &&
                    (pc.CaseId == null || pc.CaseId == caseId)
                );
            IEnumerable<PromoCode> promoCodes = await GetAllAsync(options);
            PromoCode? promoCode = promoCodes.FirstOrDefault();
            if (promoCode == null)
            {
                return null;
            }

            bool isValid = await CheckValidity(promoCode);
            return !isValid ? null : promoCode;
        }

        /// <summary>
        /// Consumes one use of a promo code and validates it afterward
        /// </summary>
        /// <param name="promoCode">The promo code to consume</param>
        /// <remarks>
        /// This method decrements RemainingUses if applicable, saves changes,
        /// and then checks the validity of the code (which may trigger deletion if no uses remain).
        /// </remarks>
        public async Task Consume(PromoCode promoCode)
        {
            if (promoCode.RemainingUses != null && promoCode.RemainingUses > 0)
            {
                promoCode.RemainingUses -= 1;
                await UpdateAsync(promoCode);
            }
            _ = await CheckValidity(promoCode);
        }
    }
}
