using S5_01_App_CS_GOAT.Services;
using S5_01_App_CS_GOAT.Models.Repository;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    public partial class PromoCode : IUserDependant, ITimedAction
    {
        public int? DependantUserId { get => this.UserId; }

        public static TimedActionFrequency TickFrequency => TimedActionFrequency.Hourly;

        public async Task Tick(IServiceScope scope)
        {
            IPromoCodeRepository promoCodeRepository = scope.ServiceProvider.GetRequiredService<IPromoCodeRepository>();
            await promoCodeRepository.CheckValidity(this);
        } 

        public bool IsExpired()
        {
            if (this.ExpiryDate == null) return false;
            return DateTime.Now > this.ExpiryDate.Value;
        }

        public DateTime? NextRefresh()
        {
            if (this.RefreshDelay == null || this.ExpiryDate == null) return null;
            return this.ExpiryDate.Value.Add(this.RefreshDelay.Value);
        }

        public bool IsDueForRefresh()
        {
            if (!this.IsExpired()) return false;
            if (this.RefreshDelay == null) return false;
            return DateTime.Now >= this.NextRefresh();
        }

        public bool IsDueForDelete()
        {
            if (this.RefreshDelay != null) return false;
            if (this.IsExpired() || this.RemainingUses == 0) return false;
            return false;
        }

        public bool IsValid()
        {
            return (
                this.ValidityStart <= DateTime.Now && !this.IsExpired() &&
                (this.RemainingUses == null || this.RemainingUses > 0)
            );
        }

        public double Apply(double value)
        {
            if (this.DiscountAmount != null)
                value -= this.DiscountAmount.Value;
            if (this.DiscountPercentage != null)
                value -= value * (this.DiscountPercentage.Value / 100.0);
            return Math.Max(0.0, value);
        }
    }
}