using Microsoft.AspNetCore.Mvc;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using S5_01_App_CS_GOAT.Services;
using Shared.DTO.Helpers;
using Stripe;
using Stripe.Checkout;

namespace S5_01_App_CS_GOAT.Controllers
{
    /// <summary>
    /// Manages Stripe payment processing for wallet top-ups and withdrawals
    /// </summary>
    [Route("api/Stripe")]
    [ApiController]
    [SetThreadPrincipal]
    public class StripeController(
        IConfiguration config,
        IUserRepository userRepository,
        IStripeRepository stripeRepository
        ) : ControllerBase
    {
        /// <summary>
        /// Create a Stripe checkout session for wallet top-up payment
        /// </summary>
        /// <param name="request">Checkout request with amount and success/cancel URLs</param>
        /// <returns>Session ID and redirect URL for Stripe Checkout</returns>
        [HttpPost("create-checkout-session")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            AuthResult authResult = JwtService.JwtAuth(config);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            User? user = await userRepository.GetByIdAsync((int)authResult.AuthUserId!);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                SessionCreateOptions options = stripeRepository.NewPaymentSession(user.UserId, request);

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { sessionId = session.Id, url = session.Url });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Webhook endpoint for Stripe event notifications (payment and payout processing)
        /// </summary>
        /// <returns>Status 200 to acknowledge receipt</returns>
        [HttpPost("webhook")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                string json;
                using (var reader = new StreamReader(HttpContext.Request.Body))
                {
                    json = await reader.ReadToEndAsync();
                }
                if (string.IsNullOrEmpty(json))
                {
                    return BadRequest("Empty body");
                }

                string? stripeSignature = Request.Headers["Stripe-Signature"].FirstOrDefault();

                if (string.IsNullOrEmpty(stripeSignature))
                {
                    return BadRequest("No signature");
                }

                string? webhookSecret = config["Stripe:WebhookSecret"];
                if (string.IsNullOrEmpty(webhookSecret))
                {
                    throw new Exception("Stripe webhook secret not configured");
                }

                Event stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    webhookSecret,
                    throwOnApiVersionMismatch: false
                );

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        await stripeRepository.HandleCheckoutSessionCompleted(stripeEvent);
                        break;

                    case "setup_intent.succeeded":
                        await stripeRepository.HandleSetupIntentSucceeded(stripeEvent);
                        break;

                    default:
                        break;
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        /// <summary>
        /// Create a Stripe setup session for payment method registration (withdrawals)
        /// </summary>
        /// <param name="request">Payout request with amount and success/cancel URLs</param>
        /// <returns>Session ID and redirect URL for Stripe payment method setup</returns>
        [HttpPost("create-payout-session")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePayoutSession([FromBody] PayoutRequest request)
        {
            AuthResult authResult = JwtService.JwtAuth(config);
            if (!authResult.IsAuthenticated)
            {
                return Unauthorized();
            }

            User? user = await userRepository.GetByIdAsync((int)authResult.AuthUserId!);
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                if (request.Amount <= 0)
                {
                    return BadRequest(new { message = "Amount must be positive" });
                }

                if (user.Wallet < request.Amount)
                {
                    return BadRequest(new { message = "Insufficient funds" });
                }

                if (request.Amount < 10)
                {
                    return BadRequest(new { message = "Minimum withdrawal is €10" });
                }

                SessionCreateOptions options = stripeRepository.NewSetupSession(user.UserId, request);

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                return Ok(new { sessionId = session.Id, url = session.Url });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Failed to create payout session" });
            }
        }

    }
}