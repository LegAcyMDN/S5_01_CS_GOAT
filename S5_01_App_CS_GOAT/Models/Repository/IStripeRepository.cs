using Shared.DTO.Helpers;
using Stripe;
using Stripe.Checkout;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Provides integration points for Stripe payments and payouts
    /// </summary>
    public interface IStripeRepository
    {
        /// <summary>
        /// Handles Stripe setup intent success events for withdrawals.
        /// </summary>
        Task HandleSetupIntentSucceeded(Event stripeEvent);

        /// <summary>
        /// Handles Stripe checkout session completed events for deposits or withdrawals.
        /// </summary>
        Task HandleCheckoutSessionCompleted(Event stripeEvent);

        /// <summary>
        /// Builds a Stripe checkout session for wallet funding.
        /// </summary>
        SessionCreateOptions NewPaymentSession(int userId, CheckoutRequest request);

        /// <summary>
        /// Builds a Stripe setup session for wallet withdrawal.
        /// </summary>
        SessionCreateOptions NewSetupSession(int userId, PayoutRequest request);
    }
}
