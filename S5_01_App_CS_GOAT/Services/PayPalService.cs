using System.Net;
using System.Text.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;
using JsonSerializer = PayPalHttp.JsonSerializer;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace S5_01_App_CS_GOAT.Services
{
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
                    ReturnUrl = returnUrl,  // ✅ Pas de paramètres ici - PayPal ajoute automatiquement ?token=ORDER_ID
                    CancelUrl = cancelUrl,   // ✅ Pas de paramètres ici
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

        /// <summary>
        /// Create a payout (withdrawal) to a PayPal account
        /// </summary>
        /// <param name="amount">Amount to send</param>
        /// <param name="userId">User ID</param>
        /// <param name="paypalEmail">Recipient PayPal email</param>
        /// <returns>Payout response</returns>
public async Task<PayPalPayoutResponse> CreatePayoutAsync(decimal amount, int userId, string paypalEmail)
{
    // Build alphanumeric IDs (no underscores, length safe)
    var ticks = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
    var payoutBatchId = $"P{userId}{ticks}"; // letters+digits only
    if (payoutBatchId.Length > 64) payoutBatchId = payoutBatchId.Substring(0, 64);

    var senderItemId = $"I{userId}{ticks}";
    if (senderItemId.Length > 64) senderItemId = senderItemId.Substring(0, 64);

    // POCO body — let the SDK serialize it
    var body = new
    {
        sender_batch_header = new
        {
            sender_batch_id = payoutBatchId,
            email_subject = "You have a withdrawal from CS:GOAT",
            email_message = "You have received a withdrawal from your CS:GOAT wallet."
        },
        items = new[]
        {
            new
            {
                recipient_type = "EMAIL",
                amount = new { value = amount.ToString("F2"), currency = "EUR" },
                receiver = paypalEmail,
                note = $"CS:GOAT withdrawal for user {userId}",
                sender_item_id = senderItemId
            }
        }
    };

    // Build request: BODY = object, and set ContentType
    var request = new PayPalHttp.HttpRequest("/v1/payments/payouts", HttpMethod.Post)
    {
        Body = body,
        ContentType = "application/json"   // <- REQUIRED by PayPalHttp encoder
    };
    // Idempotency header (nice to have)
    request.Headers.Add("PayPal-Request-Id", payoutBatchId);

    try
    {
        var response = await _client.Execute(request);          // Should be 201 on success
        var result = response.Result<JsonElement>();

        if (result.TryGetProperty("batch_header", out var batchHeader))
        {
            var createdBatchId = batchHeader.GetProperty("payout_batch_id").GetString();
            var batchStatus = batchHeader.GetProperty("batch_status").GetString();
            Console.WriteLine($"PayPal payout created: {createdBatchId} status={batchStatus}");
            return new PayPalPayoutResponse { PayoutBatchId = createdBatchId ?? payoutBatchId, Status = batchStatus ?? "PENDING" };
        }

        throw new Exception("Unexpected PayPal payout response shape.");
    }
    catch (HttpException httpEx)
    {
        // Try to extract the PayPal JSON from the HttpException message if present
        string message = httpEx.Message ?? string.Empty;
        Console.WriteLine($"PayPal HTTP error: Status={httpEx.StatusCode}; Message={message}");

        // If gateway timeout, attempt to GET the payout batch (it may have been accepted).
        if (httpEx.StatusCode == HttpStatusCode.GatewayTimeout ||
            message.IndexOf("GATEWAY_TIMEOUT", StringComparison.OrdinalIgnoreCase) >= 0)
        {
            Console.WriteLine("Gateway timeout received — verifying payout status using GET endpoint (it might have been accepted).");

            try
            {
                var checkReq = new PayPalHttp.HttpRequest($"/v1/payments/payouts/{payoutBatchId}", HttpMethod.Get);
                // GET does not require ContentType/body, but Idempotency header ok to include
                checkReq.Headers.Add("PayPal-Request-Id", payoutBatchId);

                var checkResp = await _client.Execute(checkReq);
                var checkJson = checkResp.Result<JsonElement>();
                Console.WriteLine("Payout GET result: " + checkJson.ToString());

                if (checkJson.TryGetProperty("batch_header", out var bh))
                {
                    var id = bh.GetProperty("payout_batch_id").GetString();
                    var status = bh.GetProperty("batch_status").GetString();
                    return new PayPalPayoutResponse { PayoutBatchId = id ?? payoutBatchId, Status = status ?? "UNKNOWN" };
                }
            }
            catch (Exception getEx)
            {
                Console.WriteLine("Failed to GET payout status after timeout: " + getEx.Message);
            }
        }

        // Try parsing JSON error body from httpEx.Message
        try
        {
            var doc = JsonDocument.Parse(message);
            Console.WriteLine("PayPal error details: " + doc.RootElement.ToString());
            throw new Exception($"Failed to create PayPal payout: {doc.RootElement.ToString()}", httpEx);
        }
        catch (System.Text.Json.JsonException)
        {
            // Not JSON — rethrow with the message we have
            throw new Exception($"Failed to create PayPal payout: {message}", httpEx);
        }
    }
}

}


// DTOs
public class PayoutRequestDto
{
    [JsonPropertyName("sender_batch_header")]
    public SenderBatchHeaderDto SenderBatchHeader { get; set; } = null!;
    [JsonPropertyName("items")]
    public PayoutItemDto[] Items { get; set; } = null!;
}

public class SenderBatchHeaderDto
{
    [JsonPropertyName("sender_batch_id")]
    public string SenderBatchId { get; set; } = null!;
    [JsonPropertyName("email_subject")]
    public string EmailSubject { get; set; } = null!;
    [JsonPropertyName("email_message")]
    public string EmailMessage { get; set; } = null!;
}

public class PayoutItemDto
{
    [JsonPropertyName("recipient_type")]
    public string RecipientType { get; set; } = null!;
    [JsonPropertyName("amount")]
    public PayoutAmountDto Amount { get; set; } = null!;
    [JsonPropertyName("receiver")]
    public string Receiver { get; set; } = null!;
    [JsonPropertyName("note")]
    public string Note { get; set; } = null!;
    [JsonPropertyName("sender_item_id")]
    public string SenderItemId { get; set; } = null!;
}

public class PayoutAmountDto
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = null!;
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = null!;
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

    /// <summary>
    /// Response after creating payout
    /// </summary>
    public class PayPalPayoutResponse
    {
        public string PayoutBatchId { get; set; } = null!;
        public string Status { get; set; } = null!;
    
}