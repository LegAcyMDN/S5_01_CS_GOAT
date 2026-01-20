using Microsoft.EntityFrameworkCore.Storage;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Repository;
using Shared.DTO.Helpers;
using Stripe;
using Stripe.Checkout;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Provides Stripe payment and payout helpers.
    /// </summary>
    public class StripeManager : IStripeRepository
    {
        private readonly CSGOATDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IDataRepository<MoneyTransaction, int> _transactionRepository;

        public StripeManager(
            CSGOATDbContext context,
            IUserRepository userRepository,
            IDataRepository<MoneyTransaction, int> transactionRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }

        public SessionCreateOptions NewPaymentSession(int userId, CheckoutRequest request)
        {
            return new SessionCreateOptions
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
                                UnitAmount = (long)(request.Amount * 100),
                            },
                            Quantity = 1,
                        },
                    },
                Mode = "payment",
                SuccessUrl = $"{request.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = request.CancelUrl,
                Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userId.ToString() },
                        { "amount", request.Amount.ToString() }

                    }
            };
        }

        public SessionCreateOptions NewSetupSession(int userId, PayoutRequest request)
        {
            return new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "setup",
                SuccessUrl = $"{request.SuccessUrl}?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = request.CancelUrl,
                Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userId.ToString() },
                        { "amount", request.Amount.ToString("F2") },
                        { "type", "withdrawal" }
                    }
            };
        }

        public async Task HandleSetupIntentSucceeded(Event stripeEvent)
        {
            var setupIntent = stripeEvent.Data.Object as SetupIntent;
            if (setupIntent == null || setupIntent.Metadata == null) return;

            if (!setupIntent.Metadata.ContainsKey("type")
                || setupIntent.Metadata["type"] != "withdrawal")
                    return;
            
            int userId = int.Parse(setupIntent.Metadata["user_id"]);
            double amount = double.Parse(setupIntent.Metadata["amount"]);
            
            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null || user.Wallet < amount) return;
            
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var payoutService = new PayoutService();
                var payout = await payoutService.CreateAsync(new PayoutCreateOptions
                {
                    Amount = (long)(amount * 100),
                    Currency = "eur",
                    Method = "instant",
                    SourceType = "card",
                });
                 
                user.Wallet -= amount;
                await _userRepository.UpdateAsync(user);
                  
                var moneyTrans = new MoneyTransaction
                {
                    UserId = userId,
                    WalletValue = -amount,
                    TransactionDate = DateTime.UtcNow,
                    PaymentMethodId = 1,
                };
                await _transactionRepository.AddAsync(moneyTrans);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (StripeException ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return;
            }
        }
        
        public async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;
            
            if (!session.Metadata.ContainsKey("user_id")) return;
            if (!session.Metadata.ContainsKey("amount")) return;
            
            bool withdrawal = false;
            if (session.Metadata.ContainsKey("type") &&
                session.Metadata["type"] == "withdrawal")
            {
                withdrawal = true;
            }

            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int userId = int.Parse(session.Metadata["user_id"]);
                double amount = double.Parse(session.Metadata["amount"]);
                User? user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return;

                var oldWallet = user.Wallet;
                if (!withdrawal) user.Wallet += amount;
                else
                {
                    user.Wallet -= amount;
                    amount *= -1;
                }
                await _userRepository.UpdateAsync(user);

                var moneyTrans = new MoneyTransaction
                {
                    UserId = userId,
                    WalletValue = amount,
                    TransactionDate = DateTime.UtcNow,
                    PaymentMethodId = 1,
                    CancelledOn = DateTime.UtcNow,
                    NotificationId = null

                };

                await _transactionRepository.AddAsync(moneyTrans);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine(ex.Message);
                return;
            }
        }
    }
}
