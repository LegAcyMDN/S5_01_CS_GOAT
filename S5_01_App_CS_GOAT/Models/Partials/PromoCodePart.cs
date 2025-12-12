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
            IDataRepository<PromoCode, int> promoCodeRepository = scope.ServiceProvider.GetRequiredService<IDataRepository<PromoCode, int>>();
            await this.CheckStillValid(promoCodeRepository);
        }

        public async Task<bool> CheckStillValid(IWriteRepository<PromoCode> promoCodeRepository)
        {
            if (!this.ExpiryDate.HasValue || this.ExpiryDate > DateTime.Now) return true;
            if (this.RefreshDelay.HasValue && this.ExpiryDate.Value.Add(this.RefreshDelay.Value) > DateTime.Now)
            {
                if (this.RemainingUses.HasValue && this.RemainingUses == 0)
                    this.RemainingUses = 1;
                this.ValidityStart += this.RefreshDelay.Value;
                this.ExpiryDate += this.RefreshDelay.Value;

                await promoCodeRepository.UpdateAsync(this, null);
            }
            else
            {
                await promoCodeRepository.DeleteAsync(this);
            }
            return false;
        }
    }
}