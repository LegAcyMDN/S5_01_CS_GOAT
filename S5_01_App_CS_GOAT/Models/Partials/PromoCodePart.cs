using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Models.EntityFramework
{
    /// <summary>
    /// Partial class for PromoCode that implements user-dependent and timed action functionality
    /// </summary>
    /// <remarks>
    /// This partial provides business logic for promotional codes including:
    /// - Expiry and refresh date tracking
    /// - Usage limit management
    /// - Discount calculation (fixed amount and percentage)
    /// - Periodic validity checking via the timed action system
    /// </remarks>
    public partial class PromoCode : IUserDependant, ITimedAction
    {
        /// <summary>
        /// Gets the user ID that owns this promo code
        /// </summary>
        public int? DependantUserId => UserId;

        /// <summary>
        /// Defines how frequently this promo code's validity should be checked
        /// </summary>
        /// <remarks>
        /// Promo codes are checked every hour to expire codes and trigger refreshes
        /// </remarks>
        public static TimedActionFrequency TickFrequency => TimedActionFrequency.Hourly;

        /// <summary>
        /// Executes periodic validity checks on this promo code
        /// </summary>
        /// <param name="scope">The service scope providing access to the promo code repository</param>
        /// <remarks>
        /// This method is called by the TimedActionService at the specified frequency.
        /// It delegates to the repository's CheckValidity method to handle expiry and refresh logic.
        /// </remarks>
        public async Task Tick(IServiceScope scope)
        {
            IPromoCodeRepository promoCodeRepository = scope.ServiceProvider.GetRequiredService<IPromoCodeRepository>();
            _ = await promoCodeRepository.CheckValidity(this);
        }

        /// <summary>
        /// Determines if this promo code has expired based on its expiry date
        /// </summary>
        /// <returns>True if the promo code has an expiry date and it is in the past; otherwise false</returns>
        public bool IsExpired()
        {
            return ExpiryDate == null ? false : DateTime.Now > ExpiryDate.Value;
        }

        /// <summary>
        /// Calculates the next refresh date for this promo code
        /// </summary>
        /// <returns>The next refresh date if both RefreshDelay and ExpiryDate are set; otherwise null</returns>
        /// <remarks>
        /// The refresh date is calculated by adding the RefreshDelay to the ExpiryDate.
        /// This allows expired codes to be refreshed (reset) after a certain period.
        /// </remarks>
        public DateTime? NextRefresh()
        {
            return RefreshDelay == null || ExpiryDate == null ? null : ExpiryDate.Value.Add(RefreshDelay.Value);
        }

        /// <summary>
        /// Determines if this promo code is due for refresh
        /// </summary>
        /// <returns>True if the code has expired and the next refresh date has passed; otherwise false</returns>
        /// <remarks>
        /// A code must be expired first and have a refresh delay configured before it can be refreshed.
        /// </remarks>
        public bool IsDueForRefresh()
        {
            // Code must be expired to be eligible for refresh
            if (!IsExpired())
            {
                return false;
            }

            // Must have a refresh delay and the next refresh time must have passed
            return RefreshDelay == null ? false : DateTime.Now >= NextRefresh();
        }

        /// <summary>
        /// Determines if this promo code should be deleted
        /// </summary>
        /// <returns>True if the code should be deleted; otherwise false</returns>
        /// <remarks>
        /// A code is marked for deletion if:
        /// - It has no refresh delay (cannot be refreshed) AND it is either expired or has no remaining uses
        /// Codes that can be refreshed are not deleted; they are refreshed instead.
        /// </remarks>
        public bool IsDueForDelete()
        {
            // Only delete if refresh is not possible
            return RefreshDelay != null ? false : IsExpired() || RemainingUses == 0;
        }

        /// <summary>
        /// Determines if this promo code is currently valid for use
        /// </summary>
        /// <returns>True if the code is within its validity period and has remaining uses; otherwise false</returns>
        /// <remarks>
        /// A code is valid if:
        /// - The current date is on or after the validity start date
        /// - The code has not expired
        /// - If a usage limit exists, there are remaining uses available
        /// </remarks>
        public bool IsValid()
        {
            return
                ValidityStart <= DateTime.Now && !IsExpired() &&
                (RemainingUses == null || RemainingUses > 0)
            ;
        }

        /// <summary>
        /// Applies this promo code's discount to a monetary value
        /// </summary>
        /// <param name="value">The original value to apply the discount to</param>
        /// <returns>The discounted value, with a minimum of 0.0</returns>
        /// <remarks>
        /// Discounts are applied in the following order:
        /// 1. Fixed discount amount (if configured) is subtracted
        /// 2. Percentage discount (if configured) is applied to the remaining value
        /// The result is clamped to a minimum of 0.0 to prevent negative values.
        /// </remarks>
        public double Apply(double value)
        {
            // Apply fixed discount amount if configured
            if (DiscountAmount != null)
            {
                value -= DiscountAmount.Value;
            }

            // Apply percentage discount if configured (to the remaining value after fixed discount)
            if (DiscountPercentage != null)
            {
                value -= value * (DiscountPercentage.Value / 100.0);
            }

            // Ensure the result is never negative
            return Math.Max(0.0, value);
        }
    }
}