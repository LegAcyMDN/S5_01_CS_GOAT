using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class PromoCode : IUserDependant, ITimedAction
    {
        public int? DependantUserId => UserId;

        public static TimedActionFrequency TickFrequency => TimedActionFrequency.Hourly;

        public async Task Tick(IServiceScope scope)
        {
            IPromoCodeRepository promoCodeRepository = scope.ServiceProvider.GetRequiredService<IPromoCodeRepository>();
            _ = await promoCodeRepository.CheckValidity(this);
        }

        public bool IsExpired()
        {
            return ExpiryDate == null ? false : DateTime.Now > ExpiryDate.Value;
        }

        public DateTime? NextRefresh()
        {
            return RefreshDelay == null || ExpiryDate == null ? null : ExpiryDate.Value.Add(RefreshDelay.Value);
        }

        public bool IsDueForRefresh()
        {
            if (!IsExpired())
            {
                return false;
            }

            return RefreshDelay == null ? false : DateTime.Now >= NextRefresh();
        }

        public bool IsDueForDelete()
        {
            return RefreshDelay != null ? false : IsExpired() || RemainingUses == 0;
        }

        public bool IsValid()
        {
            return
                ValidityStart <= DateTime.Now && !IsExpired() &&
                (RemainingUses == null || RemainingUses > 0)
            ;
        }

        public double Apply(double value, bool consume = false)
        {
            if (DiscountAmount != null)
            {
                value -= DiscountAmount.Value;
            }

            if (DiscountPercentage != null)
            {
                value -= value * (DiscountPercentage.Value / 100.0);
            }

            return Math.Max(0.0, value);
        }
    }
}