using PayPalCheckoutSdk.Orders;
using S5_01_App_CS_GOAT.Models.DataManager;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Provides integration points for PayPal Checkout and Payouts
    /// </summary>
    public interface IPayPalRepository
    {
        /// <summary>
        /// Creates a PayPal order to add funds to a user's wallet
        /// </summary>
        /// <param name="amount">Amount to charge in EUR</param>
        /// <param name="userId">ID of the user funding their wallet</param>
        /// <returns>Order information including approval link and status</returns>
        Task<PayPalOrderResponse> CreateOrderAsync(decimal amount, int userId);

        /// <summary>
        /// Captures a previously approved PayPal order
        /// </summary>
        /// <param name="orderId">The PayPal order identifier</param>
        /// <returns>Capture details including capture ID, status, and amount</returns>
        Task<PayPalCaptureResponse> CaptureOrderAsync(string orderId);

        /// <summary>
        /// Retrieves PayPal order details for verification
        /// </summary>
        /// <param name="orderId">The PayPal order identifier</param>
        /// <returns>Full PayPal order data</returns>
        Task<Order> GetOrderDetailsAsync(string orderId);

        /// <summary>
        /// Initiates a PayPal payout to send funds to a user's PayPal account
        /// </summary>
        /// <param name="amount">Amount to send in EUR</param>
        /// <param name="userId">ID of the user receiving the payout</param>
        /// <param name="paypalEmail">Recipient PayPal email</param>
        /// <returns>Payout batch details including status</returns>
        Task<PayPalPayoutResponse> CreatePayoutAsync(decimal amount, int userId, string paypalEmail);
    }
}
