using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/paypal")]
    [ApiController]
    [SetThreadPrincipal]
    public class PayPalController : ControllerBase
    {
        private readonly PayPalService _payPalService;
        private readonly IUserRepository _userRepository;
        private readonly IDataRepository<MoneyTransaction, int> _transactionRepository; 
        private readonly IConfiguration _configuration;

        public PayPalController(
            PayPalService payPalService,
            IUserRepository userRepository,
            IDataRepository<MoneyTransaction, int> transactionRepository,
            IConfiguration configuration)
        {
            _payPalService = payPalService;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Create a PayPal order to recharge wallet
        /// </summary>
        [HttpPost("create-order")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentDTO request)
        {
            Console.WriteLine("=== CREATE PAYPAL ORDER STARTED ===");
            
            var auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                Console.WriteLine("❌ Unauthorized");
                return Unauthorized();
            }

            var userId = (int)auth.AuthUserId;
            Console.WriteLine($"User ID: {userId}");

            // Validate amount
            if (request.Amount <= 0 || request.Amount > 1000)
            {
                Console.WriteLine($"❌ Invalid amount: {request.Amount}");
                return BadRequest("Amount must be between 0 and 1000 EUR");
            }

            try
            {
                // Create PayPal order
                Console.WriteLine($"Creating PayPal order for €{request.Amount}...");
                var orderResponse = await _payPalService.CreateOrderAsync(request.Amount, userId);
                
                Console.WriteLine($"✅ PayPal order created: {orderResponse.OrderId}");
                Console.WriteLine($"Approval URL: {orderResponse.ApprovalUrl}");

                return Ok(new
                {
                    orderId = orderResponse.OrderId,
                    approvalUrl = orderResponse.ApprovalUrl,
                    status = orderResponse.Status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error creating PayPal order: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { error = "Failed to create payment order", details = ex.Message });
            }
        }

        /// <summary>
        /// Capture payment after user approval
        /// </summary>
        [HttpPost("capture-order")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CaptureOrder([FromQuery] string orderId)
        {
            Console.WriteLine("=== CAPTURE PAYPAL ORDER STARTED ===");
            Console.WriteLine($"Order ID: {orderId}");
            
            var auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                Console.WriteLine("❌ Unauthorized");
                return Unauthorized();
            }

            var userId = (int)auth.AuthUserId;
            Console.WriteLine($"User ID: {userId}");

            if (string.IsNullOrEmpty(orderId))
            {
                Console.WriteLine("❌ No order ID provided");
                return BadRequest("Order ID is required");
            }

            try
            {
                // Get order details first to verify user
                Console.WriteLine("Fetching order details...");
                var orderDetails = await _payPalService.GetOrderDetailsAsync(orderId);
                var customId = orderDetails.PurchaseUnits[0].CustomId;
                Console.WriteLine($"Order custom ID: {customId}");

                if (customId != userId.ToString())
                {
                    Console.WriteLine($"❌ Order belongs to user {customId}, not {userId}");
                    return Forbid("This order does not belong to you");
                }

                // Capture the payment
                Console.WriteLine("Capturing payment...");
                var captureResponse = await _payPalService.CaptureOrderAsync(orderId);
                Console.WriteLine($"Capture status: {captureResponse.Status}");
                Console.WriteLine($"Capture amount: €{captureResponse.Amount}");

                if (captureResponse.Status != "COMPLETED")
                {
                    Console.WriteLine($"❌ Payment not completed: {captureResponse.Status}");
                    return BadRequest(new { error = "Payment was not completed", status = captureResponse.Status });
                }

                // Update user wallet
                Console.WriteLine($"Fetching user {userId}...");
                var user = await _userRepository.GetByIdAsyncNew(userId);
                if (user == null)
                {
                    Console.WriteLine($"❌ User {userId} not found!");
                    return NotFound("User not found");
                }

                Console.WriteLine($"Current wallet: €{user.Wallet}");
                var oldWallet = user.Wallet;
                user.Wallet += (double)captureResponse.Amount;
                Console.WriteLine($"New wallet: €{user.Wallet} (added €{captureResponse.Amount})");
                
                await _userRepository.UpdateAsync(user);
                Console.WriteLine("✅ User wallet updated");

                // Create transaction record
                Console.WriteLine("Creating transaction record...");
                var transaction = new MoneyTransaction  
                {
                    UserId = userId,      
                    WalletValue = Convert.ToDouble(captureResponse.Amount),      
                    TransactionDate = DateTime.UtcNow,      
                    PaymentMethodId = 3, // Make sure this is the correct PaymentMethod ID for PayPal
                    CancelledOn = null, // ⚠️ IMPORTANT: Don't set this! Leave it null for successful transactions
                    NotificationId = null
                }; 
            
                await _transactionRepository.AddAsync(transaction);  
                Console.WriteLine($"✅ Transaction created: ID={transaction.TransactionId}");
                
                Console.WriteLine($"✅✅✅ COMPLETED: Added €{captureResponse.Amount} to user {userId}");
                Console.WriteLine($"Old balance: €{oldWallet} → New balance: €{user.Wallet}");
                Console.WriteLine("=== CAPTURE PAYPAL ORDER ENDED ===");

                return Ok(new
                {
                    success = true,
                    amount = captureResponse.Amount,
                    oldBalance = oldWallet,
                    newBalance = user.Wallet,
                    transactionId = captureResponse.CaptureId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error capturing PayPal order: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return BadRequest(new { error = "Failed to process payment", details = ex.Message });
            }
        }

        /// <summary>
        /// Webhook endpoint for PayPal notifications (optional but recommended)
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PayPalWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var webhookPayload = await reader.ReadToEndAsync();

            Console.WriteLine($"PayPal Webhook received: {webhookPayload}");

            // TODO: Verify webhook signature
            // TODO: Process webhook events (PAYMENT.CAPTURE.COMPLETED, etc.)

            return Ok();
        }
    }
}