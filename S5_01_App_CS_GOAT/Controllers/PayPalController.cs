using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
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
        /// Create a PayPal order for adding funds to user wallet
        /// </summary>
        [HttpPost("create-order")]
        [Authorize]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentDTO request)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("CREATE PAYPAL ORDER - ADD FUNDS TO WALLET");
            Console.WriteLine("===========================================");

            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                Console.WriteLine("Unauthorized access attempt");
                return Unauthorized();
            }

            int userId = (int)auth.AuthUserId;
            Console.WriteLine("User ID: " + userId);
            Console.WriteLine("Amount requested: " + request.Amount + " EUR");

            if (request.Amount <= 0)
            {
                Console.WriteLine("Invalid amount: " + request.Amount);
                return BadRequest(new ErrorResponse
                {
                    Error = "Invalid amount",
                    Details = "Amount must be between 0 and 1000 EUR"
                });
            }

            try
            {
                var orderResponse = await _payPalService.CreateOrderAsync(request.Amount, userId);

                Console.WriteLine("PayPal order created successfully");
                Console.WriteLine("Order ID: " + orderResponse.OrderId);
                Console.WriteLine("Status: " + orderResponse.Status);
                Console.WriteLine("Redirect user to: " + orderResponse.ApprovalUrl);
                Console.WriteLine("===========================================");

                return Ok(new CreateOrderResponse
                {
                    OrderId = orderResponse.OrderId,
                    ApprovalUrl = orderResponse.ApprovalUrl,
                    Status = orderResponse.Status
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: Failed to create PayPal order");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Stack trace: " + ex.StackTrace);
                Console.WriteLine("===========================================");

                return BadRequest(new ErrorResponse
                {
                    Error = "Failed to create payment order",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Capture payment after user approval
        /// </summary>
        [HttpPost("capture-order")]
        [Authorize]
        [ProducesResponseType(typeof(CaptureOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CaptureOrder([FromQuery] string orderId)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("CAPTURE PAYPAL ORDER - USER PAID");
            Console.WriteLine("===========================================");
            Console.WriteLine("Order ID: " + orderId);

            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                Console.WriteLine("Unauthorized access attempt");
                return Unauthorized();
            }

            int userId = (int)auth.AuthUserId;
            Console.WriteLine("User ID: " + userId);

            if (string.IsNullOrEmpty(orderId))
            {
                Console.WriteLine("ERROR: No order ID provided");
                return BadRequest(new ErrorResponse
                {
                    Error = "Invalid request",
                    Details = "Order ID is required"
                });
            }

            try
            {
                Console.WriteLine("Verifying order ownership...");
                Order orderDetails = await _payPalService.GetOrderDetailsAsync(orderId);
                string customId = orderDetails.PurchaseUnits[0].CustomId;
                Console.WriteLine("Order belongs to user: " + customId);

                if (customId != userId.ToString())
                {
                    Console.WriteLine("ERROR: Order ownership mismatch");
                    Console.WriteLine("Order user: " + customId + ", Current user: " + userId);
                    return Forbid("This order does not belong to you");
                }

                Console.WriteLine("Capturing payment from PayPal...");
                var captureResponse = await _payPalService.CaptureOrderAsync(orderId);
                Console.WriteLine("Capture status: " + captureResponse.Status);
                Console.WriteLine("Captured amount: " + captureResponse.Amount + " EUR");

                if (captureResponse.Status != "COMPLETED")
                {
                    Console.WriteLine("ERROR: Payment not completed");
                    Console.WriteLine("Status: " + captureResponse.Status);
                    return BadRequest(new ErrorResponse
                    {
                        Error = "Payment was not completed",
                        Details = "Status: " + captureResponse.Status
                    });
                }

                Console.WriteLine("Loading user from database...");
                User user = await _userRepository.GetByIdAsyncNew(userId);
                if (user == null)
                {
                    Console.WriteLine("ERROR: User not found in database");
                    return NotFound(new ErrorResponse
                    {
                        Error = "User not found",
                        Details = "User ID " + userId + " does not exist"
                    });
                }

                double oldWallet = user.Wallet;
                user.Wallet += (double)captureResponse.Amount;
                Console.WriteLine("Updating wallet: " + oldWallet + " EUR -> " + user.Wallet + " EUR");

                await _userRepository.UpdateAsync(user);
                Console.WriteLine("Wallet updated successfully");

                Console.WriteLine("Creating transaction record...");
                MoneyTransaction transaction = new MoneyTransaction
                {
                    UserId = userId,
                    WalletValue = Convert.ToDouble(captureResponse.Amount),
                    TransactionDate = DateTime.UtcNow,
                    PaymentMethodId = 3,
                    CancelledOn = null,
                    NotificationId = null
                };

                await _transactionRepository.AddAsync(transaction);
                Console.WriteLine("Transaction created with ID: " + transaction.TransactionId);

                Console.WriteLine("===========================================");
                Console.WriteLine("SUCCESS: Payment captured and wallet updated");
                Console.WriteLine("User: " + userId);
                Console.WriteLine("Amount added: " + captureResponse.Amount + " EUR");
                Console.WriteLine("Old balance: " + oldWallet + " EUR");
                Console.WriteLine("New balance: " + user.Wallet + " EUR");
                Console.WriteLine("===========================================");

                return Ok(new CaptureOrderResponse
                {
                    Success = true,
                    Amount = captureResponse.Amount,
                    OldBalance = oldWallet,
                    NewBalance = user.Wallet,
                    TransactionId = captureResponse.CaptureId
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("ERROR: Failed to capture order");
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Stack trace: " + ex.StackTrace);
                Console.WriteLine("===========================================");

                return BadRequest(new ErrorResponse
                {
                    Error = "Failed to process payment",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Webhook endpoint for PayPal notifications
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PayPalWebhook()
        {
            StreamReader reader = new StreamReader(Request.Body);
            string webhookPayload = await reader.ReadToEndAsync();

            Console.WriteLine("===========================================");
            Console.WriteLine("PAYPAL WEBHOOK RECEIVED");
            Console.WriteLine("===========================================");
            Console.WriteLine("Payload: " + webhookPayload);
            Console.WriteLine("===========================================");

            return Ok();
        }

        /// <summary>
        /// Request withdrawal - SEND MONEY TO USER via PayPal Payouts
        /// </summary>
        [HttpPost("withdraw")]
        [Authorize]
        [ProducesResponseType(typeof(WithdrawalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WithdrawalPendingResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(WithdrawalErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawalRequestDTO request)
        {
            Console.WriteLine("===========================================");
            Console.WriteLine("WITHDRAWAL REQUEST - SEND MONEY TO USER");
            Console.WriteLine("===========================================");

            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                Console.WriteLine("Unauthorized access attempt");
                return Unauthorized();
            }

            int userId = (int)auth.AuthUserId;
            Console.WriteLine("User ID: " + userId);
            Console.WriteLine("Withdrawal amount: " + request.Amount + " EUR");
            Console.WriteLine("Recipient PayPal: " + request.PayPalEmail);

            // Validation
            if (request.Amount < 10)
            {
                Console.WriteLine("ERROR: Amount too low (" + request.Amount + " EUR)");
                return BadRequest(new WithdrawalErrorResponse
                {
                    Error = "Amount too low",
                    Details = "Minimum withdrawal is 10 EUR",
                    StatusCode = null
                });
            }

            if (request.Amount > 5000)
            {
                Console.WriteLine("ERROR: Amount too high (" + request.Amount + " EUR)");
                return BadRequest(new WithdrawalErrorResponse
                {
                    Error = "Amount too high",
                    Details = "Maximum withdrawal is 5000 EUR",
                    StatusCode = null
                });
            }

            if (string.IsNullOrEmpty(request.PayPalEmail) || !request.PayPalEmail.Contains("@"))
            {
                Console.WriteLine("ERROR: Invalid PayPal email: " + request.PayPalEmail);
                return BadRequest(new WithdrawalErrorResponse
                {
                    Error = "Invalid email",
                    Details = "Valid PayPal email required",
                    StatusCode = null
                });
            }

            try
            {
                Console.WriteLine("Loading user from database...");
                User user = await _userRepository.GetByIdAsyncNew(userId);
                if (user == null)
                {
                    Console.WriteLine("ERROR: User not found");
                    return NotFound(new WithdrawalErrorResponse
                    {
                        Error = "User not found",
                        Details = "User ID " + userId + " does not exist",
                        StatusCode = null
                    });
                }

                Console.WriteLine("Current wallet balance: " + user.Wallet + " EUR");

                if (user.Wallet < (double)request.Amount)
                {
                    Console.WriteLine("ERROR: Insufficient funds");
                    Console.WriteLine("Available: " + user.Wallet + " EUR");
                    Console.WriteLine("Requested: " + request.Amount + " EUR");
                    return BadRequest(new WithdrawalErrorResponse
                    {
                        Error = "Insufficient funds",
                        Details = "Current balance: " + user.Wallet + " EUR, Requested: " + request.Amount + " EUR",
                        StatusCode = null
                    });
                }

                Console.WriteLine("Requesting payout from PayPal...");
                Console.WriteLine("IMPORTANT: Payout is attempted BEFORE deducting from wallet");

                // CRITICAL: On essaie le payout AVANT de d�duire l'argent
                var payoutResponse = await _payPalService.CreatePayoutAsync(
                    request.Amount,
                    userId,
                    request.PayPalEmail);

                Console.WriteLine("PayPal payout accepted!");
                Console.WriteLine("Payout Batch ID: " + payoutResponse.PayoutBatchId);
                Console.WriteLine("Status: " + payoutResponse.Status);

                // SEULEMENT MAINTENANT on d�duit l'argent du wallet
                Console.WriteLine("Deducting amount from wallet...");
                double oldWallet = user.Wallet;
                user.Wallet -= (double)request.Amount;
                await _userRepository.UpdateAsync(user);
                Console.WriteLine("Wallet updated: " + oldWallet + " EUR -> " + user.Wallet + " EUR");

                Console.WriteLine("Creating transaction record...");
                MoneyTransaction transaction = new MoneyTransaction
                {
                    UserId = userId,
                    WalletValue = -(double)request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    PaymentMethodId = 3,
                    CancelledOn = null,
                    NotificationId = null
                };

                await _transactionRepository.AddAsync(transaction);
                Console.WriteLine("Transaction created with ID: " + transaction.TransactionId);

                Console.WriteLine("===========================================");
                Console.WriteLine("SUCCESS: Withdrawal completed");
                Console.WriteLine("User: " + userId);
                Console.WriteLine("Amount sent: " + request.Amount + " EUR");
                Console.WriteLine("Recipient: " + request.PayPalEmail);
                Console.WriteLine("Old balance: " + oldWallet + " EUR");
                Console.WriteLine("New balance: " + user.Wallet + " EUR");
                Console.WriteLine("PayPal Batch ID: " + payoutResponse.PayoutBatchId);
                Console.WriteLine("===========================================");

                return Ok(new WithdrawalResponse
                {
                    Success = true,
                    Message = "Withdrawal sent to your PayPal account. You should receive it within a few minutes.",
                    Amount = request.Amount,
                    OldBalance = oldWallet,
                    NewBalance = user.Wallet,
                    PaypalEmail = request.PayPalEmail,
                    PayoutBatchId = payoutResponse.PayoutBatchId,
                    PayoutStatus = payoutResponse.Status,
                    TransactionId = transaction.TransactionId
                });
            }
            catch (PayPalTimeoutException timeoutEx)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("TIMEOUT: PayPal is processing");
                Console.WriteLine("===========================================");
                Console.WriteLine("Message: " + timeoutEx.Message);
                Console.WriteLine("Batch ID: " + timeoutEx.PayoutBatchId);
                Console.WriteLine("IMPORTANT: Money was NOT deducted from wallet");
                Console.WriteLine("User needs to verify manually if money was sent");
                Console.WriteLine("===========================================");

                return StatusCode(StatusCodes.Status202Accepted, new WithdrawalPendingResponse
                {
                    Success = false,
                    Pending = true,
                    Message = timeoutEx.Message,
                    PayoutBatchId = timeoutEx.PayoutBatchId,
                    Instructions = "Please wait 5-10 minutes and check your PayPal account. " +
                                   "If you don't receive the money, contact support with this batch ID: " + timeoutEx.PayoutBatchId
                });
            }
            catch (HttpException ex)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("PAYPAL API ERROR");
                Console.WriteLine("===========================================");
                Console.WriteLine("Status Code: " + ex.StatusCode);
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Money was NOT deducted from wallet");
                Console.WriteLine("===========================================");

                return BadRequest(new WithdrawalErrorResponse
                {
                    Error = "PayPal withdrawal failed",
                    Details = ex.Message,
                    StatusCode = ex.StatusCode.ToString()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("===========================================");
                Console.WriteLine("UNEXPECTED ERROR");
                Console.WriteLine("===========================================");
                Console.WriteLine("Type: " + ex.GetType().Name);
                Console.WriteLine("Message: " + ex.Message);
                Console.WriteLine("Stack trace: " + ex.StackTrace);
                Console.WriteLine("Money was NOT deducted from wallet");
                Console.WriteLine("===========================================");

                return BadRequest(new WithdrawalErrorResponse
                {
                    Error = "Withdrawal failed",
                    Details = ex.Message,
                    StatusCode = null
                });
            }
        }
    }

    #region DTOs

    public class WithdrawalRequestDTO
    {
        public decimal Amount { get; set; }
        public string PayPalEmail { get; set; } = string.Empty;
    }

    public class WithdrawalResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public string PaypalEmail { get; set; } = string.Empty;
        public string PayoutBatchId { get; set; } = string.Empty;
        public string PayoutStatus { get; set; } = string.Empty;
        public int TransactionId { get; set; }
    }

    public class WithdrawalPendingResponse
    {
        public bool Success { get; set; }
        public bool Pending { get; set; }
        public string Message { get; set; } = string.Empty;
        public string PayoutBatchId { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
    }

    public class WithdrawalErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string? StatusCode { get; set; }
    }

    public class CreateOrderResponse
    {
        public string OrderId { get; set; } = string.Empty;
        public string ApprovalUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class CaptureOrderResponse
    {
        public bool Success { get; set; }
        public decimal Amount { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }

    public class ErrorResponse
    {
        public string Error { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    #endregion
}