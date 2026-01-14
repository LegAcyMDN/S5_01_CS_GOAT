using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;

namespace S5_01_App_CS_GOAT.Services
{
    /// <summary>
    /// Provides integration with PayPal Checkout API for payment processing
    /// </summary>
    public class PayPalService
    {
        private readonly PayPalHttpClient _client;
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            // Configure PayPal environment
            var environment = GetEnvironment();
            _client = new PayPalHttpClient(environment);
        }

        /// <summary>
        /// Get PayPal environment (Sandbox or Live)
        /// </summary>
        private PayPalEnvironment GetEnvironment()
        {
            var clientId = _configuration["PayPal:ClientId"];
            var clientSecret = _configuration["PayPal:ClientSecret"];
            var mode = _configuration["PayPal:Mode"]; // "sandbox" or "live"

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("PayPal credentials not configured");
            }

            return mode?.ToLower() == "live"
                ? new LiveEnvironment(clientId, clientSecret)
                : new SandboxEnvironment(clientId, clientSecret);
        }

        /// <summary>
        /// Create a PayPal order for wallet recharge
        /// </summary>
        /// <param name="amount">Amount in EUR</param>
        /// <param name="userId">User ID for reference</param>
        /// <returns>Order ID and approval URL</returns>
        public async Task<PayPalOrderResponse> CreateOrderAsync(decimal amount, int userId)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(amount, userId));

            try
            {
                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                // Get approval URL
                var approvalUrl = result.Links
                    .FirstOrDefault(link => link.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))
                    ?.Href;

                return new PayPalOrderResponse
                {
                    OrderId = result.Id,
                    ApprovalUrl = approvalUrl,
                    Status = result.Status
                };
            }
            catch (HttpException ex)
            {
                var error = ex.Message;
                Console.WriteLine($"PayPal Error: {error}");
                throw new Exception($"Failed to create PayPal order: {error}");
            }
        }

        /// <summary>
        /// Capture payment after user approval
        /// </summary>
        /// <param name="orderId">PayPal order ID</param>
        /// <returns>Capture details</returns>
        public async Task<PayPalCaptureResponse> CaptureOrderAsync(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                var response = await _client.Execute(request);
                var result = response.Result<Order>();

                var capture = result.PurchaseUnits[0].Payments.Captures[0];

                return new PayPalCaptureResponse
                {
                    OrderId = result.Id,
                    CaptureId = capture.Id,
                    Status = capture.Status,
                    Amount = decimal.Parse(capture.Amount.Value),
                    Currency = capture.Amount.CurrencyCode
                };
            }
            catch (HttpException ex)
            {
                var error = ex.Message;
                Console.WriteLine($"PayPal Capture Error: {error}");
                throw new Exception($"Failed to capture PayPal order: {error}");
            }
        }

        /// <summary>
        /// Build PayPal order request body
        /// </summary>
        private OrderRequest BuildRequestBody(decimal amount, int userId)
        {
            var returnUrl = _configuration["PayPal:ReturnUrl"] ?? "https://localhost:7030/payment/success";
            var cancelUrl = _configuration["PayPal:CancelUrl"] ?? "https://localhost:7030/payment/cancel";

            return new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = $"{returnUrl}?userId={userId}",
                    CancelUrl = $"{cancelUrl}?userId={userId}",
                    BrandName = "CS:GOAT",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId = $"USER_{userId}_{DateTime.UtcNow.Ticks}",
                        Description = "CS:GOAT Wallet Recharge",
                        CustomId = userId.ToString(),
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "EUR",
                            Value = amount.ToString("F2")
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Verify order details
        /// </summary>
        /// <param name="orderId">PayPal order ID</param>
        /// <returns>Order details</returns>
        public async Task<Order> GetOrderDetailsAsync(string orderId)
        {
            var request = new OrdersGetRequest(orderId);
            
            try
            {
                var response = await _client.Execute(request);
                return response.Result<Order>();
            }
            catch (HttpException ex)
            {
                Console.WriteLine($"PayPal Error: {ex.Message}");
                throw new Exception($"Failed to get order details: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Response after creating PayPal order
    /// </summary>
    public class PayPalOrderResponse
    {
        public string OrderId { get; set; } = null!;
        public string? ApprovalUrl { get; set; }
        public string Status { get; set; } = null!;
    }

    /// <summary>
    /// Response after capturing payment
    /// </summary>
    public class PayPalCaptureResponse
    {
        public string OrderId { get; set; } = null!;
        public string CaptureId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
    }
}