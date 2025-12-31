using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;

namespace S5_01_App_CS_GOAT.Controllers
{ // TODO REFACTO CETTE HORREUR
    [Route("api/[controller]")]
    [ApiController]
    [SetThreadPrincipal]
    public class StripeController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IDataRepository<MoneyTransaction, int> _transactionRepository;

        public StripeController(
            IConfiguration config,
            IUserRepository userRepository,
            IDataRepository<MoneyTransaction, int> transactionRepository)
        {
            _config = config;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            try
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "eur",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Wallet Credit",
                                    Description = $"Add €{request.Amount} to wallet"
                                },
                                UnitAmount = (long)(request.Amount * 100), // Stripe uses cents
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = $"{request.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = request.CancelUrl,
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", request.UserId.ToString() },
                        { "amount", request.Amount.ToString() }
                    }
                };

                var service = new SessionService();
                var session = await service.CreateAsync(options);

                return Ok(new { sessionId = session.Id, url = session.Url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

[HttpPost("webhook")]
public async Task<IActionResult> Webhook()
{
    Console.WriteLine("=== WEBHOOK RECEIVED ===");
    
    var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
    var stripeSignature = Request.Headers["Stripe-Signature"].ToString();

    if (string.IsNullOrEmpty(stripeSignature))
        return BadRequest("No signature");

    try
    {
        var webhookSecret = _config["Stripe:WebhookSecret"];
        var stripeEvent = EventUtility.ConstructEvent(
            json,
            stripeSignature,
            webhookSecret,
            throwOnApiVersionMismatch: false
        );

        Console.WriteLine($"✅ Event verified: {stripeEvent.Type}");

        switch (stripeEvent.Type)
        {
            case "checkout.session.completed":
                await HandleCheckoutSessionCompleted(stripeEvent);
                break;

            case "setup_intent.succeeded": // For withdrawal card setup
                await HandleSetupIntentSucceeded(stripeEvent);
                break;

            default:
                Console.WriteLine($"⚠️ Unhandled event type: {stripeEvent.Type}");
                break;
        }

        return Ok();
    }
    catch (StripeException e)
    {
        Console.WriteLine($"❌ Stripe webhook error: {e.Message}");
        return BadRequest(e.Message);
    }
}

private async Task HandleSetupIntentSucceeded(Event stripeEvent)
{
    Console.WriteLine(">>> HandleSetupIntentSucceeded STARTED");
    
    var setupIntent = stripeEvent.Data.Object as SetupIntent;
    if (setupIntent == null || setupIntent.Metadata == null)
    {
        Console.WriteLine("❌ SetupIntent or metadata is null!");
        return;
    }

    if (!setupIntent.Metadata.ContainsKey("type") || setupIntent.Metadata["type"] != "withdrawal")
    {
        Console.WriteLine("Not a withdrawal setup");
        return;
    }

    var userId = int.Parse(setupIntent.Metadata["user_id"]);
    var amount = double.Parse(setupIntent.Metadata["amount"]);

    Console.WriteLine($"Processing withdrawal: userId={userId}, amount={amount}");

    var user = await _userRepository.GetByIdAsync(userId);
    if (user == null)
    {
        Console.WriteLine($"❌ User {userId} not found!");
        return;
    }

    if (user.Wallet < amount)
    {
        Console.WriteLine($"❌ Insufficient funds!");
        return;
    }

    // Create payout to the saved payment method
    try
    {
        var payoutService = new PayoutService();
        var payout = await payoutService.CreateAsync(new PayoutCreateOptions
        {
            Amount = (long)(amount * 100),
            Currency = "eur",
            Method = "instant", // Instant payout to debit card
            SourceType = "card",
        });

        Console.WriteLine($"✅ Payout created: {payout.Id}");

        // Deduct from wallet
        user.Wallet -= amount;
        await _userRepository.UpdateAsync(user);

        // Create transaction record
        var transaction = new MoneyTransaction
        {
            UserId = userId,
            WalletValue = -amount,
            TransactionDate = DateTime.UtcNow,
            PaymentMethodId = 1,
        };
        await _transactionRepository.AddAsync(transaction);

        Console.WriteLine($"✅✅✅ COMPLETED: Withdrew €{amount} from user {userId}. New balance: €{user.Wallet}");
    }
    catch (StripeException ex)
    {
        Console.WriteLine($"❌ Payout failed: {ex.Message}");
    }
}

private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
{
    Console.WriteLine(">>> HandleCheckoutSessionCompleted STARTED");
    
    var session = stripeEvent.Data.Object as Session;

    if (session == null)
    {
        Console.WriteLine("❌ Session is null!");
        return;
    }

    Console.WriteLine($"Session ID: {session.Id}");
    Console.WriteLine($"Payment Status: {session.PaymentStatus}");
    Console.WriteLine($"Metadata count: {session.Metadata?.Count ?? 0}");
    
    foreach (var kvp in session.Metadata ?? new Dictionary<string, string>())
    {
        Console.WriteLine($"  Metadata: {kvp.Key} = {kvp.Value}");
    }

    if (!session.Metadata.ContainsKey("user_id"))
    {
        Console.WriteLine("❌ No user_id in metadata!");
        return;
    }

    if (!session.Metadata.ContainsKey("amount"))
    {
        Console.WriteLine("❌ No amount in metadata!");
        return;
    }

    bool withdrawal = false;
    if (session.Metadata.ContainsKey("type"))
    {
        if (session.Metadata["type"] == "withdrawal")
        {
            withdrawal = true;
        }
    }

    var userId = int.Parse(session.Metadata["user_id"]);
    var amount = double.Parse(session.Metadata["amount"]);

    Console.WriteLine($"Parsed: userId={userId}, amount={amount}");

    // Get user
    Console.WriteLine($"Fetching user {userId}...");
    var user = await _userRepository.GetByIdAsync(userId);
    
    if (user == null)
    {
        Console.WriteLine($"❌ User {userId} not found in database!");
        return;
    }

    Console.WriteLine($"✅ User found: {user.Login}, current wallet: €{user.Wallet}");

    // Update wallet
    var oldWallet = user.Wallet;
    if (!withdrawal)
    {
        user.Wallet += amount;
    }
    else
    {
        user.Wallet -= amount;
        amount *= -1;
    }

    Console.WriteLine($"Updating wallet: €{oldWallet} → €{user.Wallet}");
    
    await _userRepository.UpdateAsync(user);
    Console.WriteLine("✅ User updated");

    // Create transaction record
    Console.WriteLine("Creating transaction...");
    var transaction = new MoneyTransaction
    {
        UserId = userId,
        WalletValue = amount,
        TransactionDate = DateTime.UtcNow,
        PaymentMethodId = 1,
        CancelledOn = DateTime.UtcNow,
        NotificationId = null
    };

    await _transactionRepository.AddAsync(transaction);
    Console.WriteLine($"✅ Transaction created: ID={transaction.TransactionId}");

    Console.WriteLine($"✅✅✅ COMPLETED: Added €{amount} to user {userId}. New balance: €{user.Wallet}");
    Console.WriteLine(">>> HandleCheckoutSessionCompleted ENDED");
}

[HttpPost("create-payout-session")]
public async Task<IActionResult> CreatePayoutSession([FromBody] PayoutRequest request)
{
    try
    {
        // Verify user authentication
        var userId = GetUserIdFromJwt(); // Your JWT helper method
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null) return NotFound(new { message = "User not found" });
        
        // Validate withdrawal amount
        if (request.Amount <= 0)
            return BadRequest(new { message = "Amount must be positive" });
            
        if (user.Wallet < request.Amount)
            return BadRequest(new { message = "Insufficient funds" });
            
        if (request.Amount < 10)
            return BadRequest(new { message = "Minimum withdrawal is €10" });

        // Create Stripe Checkout Session for payout
        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            Mode = "setup", // Setup mode to save card for payouts
            SuccessUrl = $"{request.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
            CancelUrl = request.CancelUrl,
            Metadata = new Dictionary<string, string>
            {
                { "user_id", userId.ToString() },
                { "amount", request.Amount.ToString("F2") },
                { "type", "withdrawal" } // Important: distinguish from deposits
            }
        };

        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return Ok(new { sessionId = session.Id, url = session.Url });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Payout session error: {ex.Message}");
        return BadRequest(new { message = "Failed to create payout session" });
    }
}

public class PayoutRequest
{
    public int UserId { get; set; }
    public double Amount { get; set; }
    public string SuccessUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}

private int GetUserIdFromJwt()
{
    var userIdClaim = User.FindFirst("userId") ?? User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
    {
        return userId;
    }
    throw new UnauthorizedAccessException("Invalid token");
}
    }

    public class CheckoutRequest
    {
        public int UserId { get; set; }
        public double Amount { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}