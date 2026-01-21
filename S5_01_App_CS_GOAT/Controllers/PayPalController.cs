using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;
using PayPalHttp;
using S5_01_App_CS_GOAT.Models.DataManager;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Controllers
{
    [Route("api/Paypal")]
    [ApiController]
    [SetThreadPrincipal]
    public class PayPalController : ControllerBase
    {
        private readonly IPayPalRepository _payPalRepository;
        private readonly IUserRepository _userRepository;
        private readonly IDataRepository<MoneyTransaction, int> _transactionRepository;
        private readonly IConfiguration _configuration;

        public PayPalController(
            IPayPalRepository payPalRepository,
            IUserRepository userRepository,
            IDataRepository<MoneyTransaction, int> transactionRepository,
            IConfiguration configuration)
        {
            _payPalRepository = payPalRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Create a PayPal order for adding funds to user wallet
        /// </summary>
        [HttpPost("create-order")]
        [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateOrder([FromBody] CreatePaymentDTO request)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId = (int)auth.AuthUserId;

            if (request.Amount <= 0)
            {
                return BadRequest("Invalid amount");
            }

            try
            {
                PayPalOrderResponse orderResponse =
                    await _payPalRepository.CreateOrderAsync(request.Amount, userId);

                return Ok(new CreateOrderResponse
                {
                    OrderId = orderResponse.OrderId,
                    ApprovalUrl = orderResponse.ApprovalUrl,
                    Status = orderResponse.Status
                });
            }
            catch (Exception)
            {
                return BadRequest("Failed to create payment order");
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
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId = (int)auth.AuthUserId;

            if (string.IsNullOrEmpty(orderId))
            {
                return BadRequest("Order ID is required");
            }

            try
            {
                Order orderDetails = await _payPalRepository.GetOrderDetailsAsync(orderId);
                string customId = orderDetails.PurchaseUnits[0].CustomId;

                if (customId != userId.ToString())
                {
                    return Forbid();
                }

                PayPalCaptureResponse captureResponse =
                    await _payPalRepository.CaptureOrderAsync(orderId);

                if (captureResponse.Status != "COMPLETED")
                {
                    return BadRequest("Payment was not completed");
                }

                User? user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                double oldWallet = user.Wallet;
                user.Wallet += (double)captureResponse.Amount;

                await _userRepository.UpdateAsync(user);

                MoneyTransaction transaction = new()
                {
                    UserId = userId,
                    WalletValue = Convert.ToDouble(captureResponse.Amount),
                    TransactionDate = DateTime.UtcNow,
                    PaymentMethodId = 3
                };

                _ = await _transactionRepository.AddAsync(transaction);

                return Ok(new CaptureOrderResponse
                {
                    Success = true,
                    Amount = captureResponse.Amount,
                    OldBalance = oldWallet,
                    NewBalance = user.Wallet,
                    TransactionId = captureResponse.CaptureId
                });
            }
            catch (Exception)
            {
                return BadRequest("Failed to process payment");
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
            StreamReader reader = new(Request.Body);

            _ = await reader.ReadToEndAsync();
            return Ok();
        }

        /// <summary>
        /// Request withdrawal - SEND MONEY TO USER via PayPal Payouts
        /// </summary>
        [HttpPost("withdraw")]
        [Authorize]
        [ProducesResponseType(typeof(WithdrawalResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(WithdrawalPendingResponse), StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> WithdrawFunds([FromBody] WithdrawalRequestDTO request)
        {
            AuthResult auth = JwtService.JwtAuth(_configuration);
            if (!auth.IsAuthenticated)
            {
                return Unauthorized();
            }

            int userId = (int)auth.AuthUserId;

            if (request.Amount < 10 || request.Amount > 5000)
            {
                return BadRequest("Invalid amount");
            }

            if (string.IsNullOrEmpty(request.PayPalEmail) || !request.PayPalEmail.Contains('@'))
            {
                return BadRequest("Invalid PayPal email");
            }

            try
            {
                User? user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return NotFound();
                }

                if (user.Wallet < (double)request.Amount)
                {
                    return BadRequest("Insufficient funds");
                }

                PayPalPayoutResponse payoutResponse = await _payPalRepository.CreatePayoutAsync(
                    request.Amount,
                    userId,
                    request.PayPalEmail);

                double oldWallet = user.Wallet;
                user.Wallet -= (double)request.Amount;
                await _userRepository.UpdateAsync(user);

                MoneyTransaction transaction = new()
                {
                    UserId = userId,
                    WalletValue = -(double)request.Amount,
                    TransactionDate = DateTime.UtcNow,
                    PaymentMethodId = 3,
                    CancelledOn = null,
                    NotificationId = null
                };

                _ = await _transactionRepository.AddAsync(transaction);

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
            catch (HttpException)
            {
                return BadRequest("PayPal withdrawal failed");
            }
            catch (Exception)
            {
                return BadRequest("Withdrawal failed");
            }
        }
    }
}
