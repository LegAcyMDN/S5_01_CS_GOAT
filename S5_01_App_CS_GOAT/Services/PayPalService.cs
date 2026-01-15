using System.Net;
using System.Text.Json;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalCheckoutSdk.Payments;
using PayPalHttp;
using System.Text.Json.Serialization;

namespace S5_01_App_CS_GOAT.Services
{
    public class PayPalService
    {
        private readonly PayPalHttpClient _client;
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
            
            PayPalEnvironment environment = GetEnvironment();
            _client = new PayPalHttpClient(environment);
        }

        /// <summary>
        /// Get PayPal environment (Sandbox or Live)
        /// </summary>
        private PayPalEnvironment GetEnvironment()
        {
            string clientId = _configuration["PayPal:ClientId"];
            string clientSecret = _configuration["PayPal:ClientSecret"];
            string mode = _configuration["PayPal:Mode"];

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
        public async Task<PayPalOrderResponse> CreateOrderAsync(decimal amount, int userId)
        {
            OrdersCreateRequest request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(BuildRequestBody(amount, userId));

            try
            {
                PayPalHttp.HttpResponse response = await _client.Execute(request);
                Order result = response.Result<Order>();

                string approvalUrl = result.Links
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
                string error = ex.Message;
                Console.WriteLine("PayPal Error: " + error);
                throw new Exception("Failed to create PayPal order: " + error);
            }
        }

        /// <summary>
        /// Capture payment after user approval
        /// </summary>
        public async Task<PayPalCaptureResponse> CaptureOrderAsync(string orderId)
        {
            OrdersCaptureRequest request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            try
            {
                PayPalHttp.HttpResponse response = await _client.Execute(request);
                Order result = response.Result<Order>();

                PayPalCheckoutSdk.Orders.Capture capture = result.PurchaseUnits[0].Payments.Captures[0];

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
                string error = ex.Message;
                Console.WriteLine("PayPal Capture Error: " + error);
                throw new Exception("Failed to capture PayPal order: " + error);
            }
        }

        /// <summary>
        /// Build PayPal order request body
        /// </summary>
        private OrderRequest BuildRequestBody(decimal amount, int userId)
        {
            string returnUrl = _configuration["PayPal:ReturnUrl"] ?? "https://localhost:7030/payment/success";
            string cancelUrl = _configuration["PayPal:CancelUrl"] ?? "https://localhost:7030/payment/cancel";

            return new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl,
                    BrandName = "CS:GOAT",
                    LandingPage = "BILLING",
                    UserAction = "PAY_NOW"
                },
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        ReferenceId = "USER_" + userId + "_" + DateTime.UtcNow.Ticks,
                        Description = "CS:GOAT Wallet Recharge",
                        CustomId = userId.ToString(),
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "EUR",
                            Value = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Verify order details
        /// </summary>
        public async Task<Order> GetOrderDetailsAsync(string orderId)
        {
            OrdersGetRequest request = new OrdersGetRequest(orderId);
            
            try
            {
                PayPalHttp.HttpResponse response = await _client.Execute(request);
                return response.Result<Order>();
            }
            catch (HttpException ex)
            {
                Console.WriteLine("PayPal Error: " + ex.Message);
                throw new Exception("Failed to get order details: " + ex.Message);
            }
        }

        /// <summary>
        /// Create a payout (withdrawal) - ENVOYER DE L'ARGENT à l'utilisateur
        /// </summary>
        public async Task<PayPalPayoutResponse> CreatePayoutAsync(decimal amount, int userId, string paypalEmail)
        {
            long ticks = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            string payoutBatchId = "P" + userId + ticks;
            if (payoutBatchId.Length > 64) 
            {
                payoutBatchId = payoutBatchId.Substring(0, 64);
            }

            string senderItemId = "I" + userId + ticks;
            if (senderItemId.Length > 64) 
            {
                senderItemId = senderItemId.Substring(0, 64);
            }

            Console.WriteLine("========================================");
            Console.WriteLine("CREATING PAYOUT");
            Console.WriteLine("========================================");
            Console.WriteLine("Batch ID: " + payoutBatchId);
            Console.WriteLine("Item ID: " + senderItemId);
            Console.WriteLine("Recipient: " + paypalEmail);
            Console.WriteLine("Amount: " + amount + " EUR");

            PayoutRequestDto body = new PayoutRequestDto
            {
                SenderBatchHeader = new SenderBatchHeaderDto
                {
                    SenderBatchId = payoutBatchId,
                    EmailSubject = "You have money from CS:GOAT",
                    EmailMessage = "You have received a withdrawal from your CS:GOAT wallet."
                },
                Items = new PayoutItemDto[]
                {
                    new PayoutItemDto
                    {
                        RecipientType = "EMAIL",
                        Amount = new PayoutAmountDto 
                        { 
                            Value = amount.ToString("F2", System.Globalization.CultureInfo.InvariantCulture), 
                            Currency = "EUR" 
                        },
                        Receiver = paypalEmail,
                        Note = "CS:GOAT withdrawal",
                        SenderItemId = senderItemId
                    }
                }
            };

            PayPalHttp.HttpRequest request = new PayPalHttp.HttpRequest("/v1/payments/payouts", HttpMethod.Post)
            {
                Body = body,
                ContentType = "application/json"
            };
            request.Headers.Add("PayPal-Request-Id", payoutBatchId);

            try
            {
                Console.WriteLine("Sending payout request to PayPal API...");
                PayPalHttp.HttpResponse response = await _client.Execute(request);
                JsonElement result = response.Result<JsonElement>();

                Console.WriteLine("PayPal Response Received:");
                Console.WriteLine(result.ToString());

                if (result.TryGetProperty("batch_header", out JsonElement batchHeader))
                {
                    string createdBatchId = batchHeader.GetProperty("payout_batch_id").GetString();
                    string batchStatus = batchHeader.GetProperty("batch_status").GetString();
                    
                    Console.WriteLine("========================================");
                    Console.WriteLine("PAYOUT CREATED SUCCESSFULLY");
                    Console.WriteLine("========================================");
                    Console.WriteLine("PayPal Batch ID: " + createdBatchId);
                    Console.WriteLine("Status: " + batchStatus);
                    Console.WriteLine("========================================");
                    
                    return new PayPalPayoutResponse 
                    { 
                        PayoutBatchId = createdBatchId ?? payoutBatchId, 
                        Status = batchStatus ?? "PENDING" 
                    };
                }

                Console.WriteLine("ERROR: Unexpected response structure from PayPal");
                throw new Exception("Unexpected PayPal payout response structure");
            }
            catch (HttpException httpEx)
            {
                string message = httpEx.Message ?? string.Empty;
                
                Console.WriteLine("========================================");
                Console.WriteLine("PAYPAL HTTP ERROR");
                Console.WriteLine("========================================");
                Console.WriteLine("Status Code: " + httpEx.StatusCode);
                Console.WriteLine("Message: " + message);
                Console.WriteLine("========================================");

                if (httpEx.StatusCode == HttpStatusCode.GatewayTimeout ||
                    message.IndexOf("GATEWAY_TIMEOUT", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Console.WriteLine("GATEWAY TIMEOUT DETECTED");
                    Console.WriteLine("Attempting to verify payout was created...");
                    
                    await Task.Delay(50000);
                    
                    try
                    {
                        PayPalHttp.HttpRequest checkReq = new PayPalHttp.HttpRequest(
                            "/v1/payments/payouts/" + payoutBatchId, 
                            HttpMethod.Get
                        );

                        PayPalHttp.HttpResponse checkResp = await _client.Execute(checkReq);
                        JsonElement checkJson = checkResp.Result<JsonElement>();

                        if (checkJson.TryGetProperty("batch_header", out JsonElement bh))
                        {
                            string id = bh.GetProperty("payout_batch_id").GetString();
                            string status = bh.GetProperty("batch_status").GetString();
                            
                            Console.WriteLine("SUCCESS: Payout was created despite timeout!");
                            Console.WriteLine("Batch ID: " + id);
                            Console.WriteLine("Status: " + status);
                            
                            return new PayPalPayoutResponse 
                            { 
                                PayoutBatchId = id ?? payoutBatchId, 
                                Status = status ?? "PENDING" 
                            };
                        }
                    }
                    catch (Exception verifyEx)
                    {
                        Console.WriteLine("Could not verify payout: " + verifyEx.Message);
                    }
                    
                    Console.WriteLine("DO NOT DEDUCT MONEY - Wait for manual verification");
                    throw new PayPalTimeoutException(
                        "PayPal is processing your withdrawal. This may take a few minutes. " +
                        "Please check your PayPal account in 5-10 minutes. " +
                        "If you don't receive the money within 24 hours, contact support with batch ID: " + payoutBatchId,
                        payoutBatchId
                    );
                }

                try
                {
                    JsonDocument doc = JsonDocument.Parse(message);
                    string errorName = "UNKNOWN";
                    string errorMessage = "Unknown error";
                    string errorDebugId = "N/A";
                    string errorDetails = "";

                    if (doc.RootElement.TryGetProperty("name", out JsonElement nameElement))
                    {
                        errorName = nameElement.GetString() ?? "UNKNOWN";
                    }
                    if (doc.RootElement.TryGetProperty("message", out JsonElement msgElement))
                    {
                        errorMessage = msgElement.GetString() ?? "Unknown error";
                    }
                    if (doc.RootElement.TryGetProperty("debug_id", out JsonElement debugElement))
                    {
                        errorDebugId = debugElement.GetString() ?? "N/A";
                    }
                    if (doc.RootElement.TryGetProperty("details", out JsonElement detailsElement))
                    {
                        errorDetails = detailsElement.ToString();
                    }

                    Console.WriteLine("PayPal Error Details:");
                    Console.WriteLine("- Name: " + errorName);
                    Console.WriteLine("- Message: " + errorMessage);
                    Console.WriteLine("- Debug ID: " + errorDebugId);
                    if (!string.IsNullOrEmpty(errorDetails))
                    {
                        Console.WriteLine("- Details: " + errorDetails);
                    }

                    if (errorName == "INSUFFICIENT_FUNDS")
                    {
                        throw new Exception("Your PayPal Business account has insufficient funds. Please add money to your PayPal account to process payouts.");
                    }
                    if (errorName == "PERMISSION_DENIED" || errorName == "UNAUTHORIZED")
                    {
                        throw new Exception("Payouts feature is not enabled for your PayPal account. Please contact PayPal to enable Payouts API access.");
                    }
                    if (errorName == "INVALID_ACCOUNT_STATUS")
                    {
                        throw new Exception("The recipient's PayPal account (" + paypalEmail + ") cannot receive money. The account may be unverified, limited, or restricted.");
                    }
                    if (errorName == "RECEIVER_UNREGISTERED")
                    {
                        throw new Exception("The PayPal account (" + paypalEmail + ") does not exist or is not registered.");
                    }
                    if (errorName == "VALIDATION_ERROR")
                    {
                        throw new Exception("PayPal validation error: " + errorMessage + ". Details: " + errorDetails);
                    }

                    throw new Exception("PayPal Error [" + errorName + "]: " + errorMessage + " (Debug ID: " + errorDebugId + ")");
                }
                catch (JsonException)
                {
                    throw new Exception("PayPal API Error: " + message);
                }
            }
            catch (Exception ex) when (ex is not PayPalTimeoutException)
            {
                Console.WriteLine("========================================");
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine("========================================");
                Console.WriteLine("Type: " + ex.GetType().Name);
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
                Console.WriteLine("========================================");
                throw;
            }
        }
    }

    #region DTOs

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

    public class PayPalOrderResponse
    {
        public string OrderId { get; set; } = null!;
        public string? ApprovalUrl { get; set; }
        public string Status { get; set; } = null!;
    }

    public class PayPalCaptureResponse
    {
        public string OrderId { get; set; } = null!;
        public string CaptureId { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
    }

    public class PayPalPayoutResponse
    {
        public string PayoutBatchId { get; set; } = null!;
        public string Status { get; set; } = null!;
    }

    public class PayPalTimeoutException : Exception
    {
        public string PayoutBatchId { get; }

        public PayPalTimeoutException(string message, string payoutBatchId) : base(message)
        {
            PayoutBatchId = payoutBatchId;
        }
    }

    #endregion
}