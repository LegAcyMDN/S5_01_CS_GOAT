using System.Net.Http.Headers;
using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Wallet avec support Stripe et PayPal
    /// </summary>
    public class WalletViewModel : ViewModelBase
    {
        private readonly IService<MoneyTransactionDTO> _moneyTransactionRepository;
        private readonly AuthService _authService;
        private readonly StripeService _stripeService;
        private readonly NavigationService _navigationService;
        private readonly HttpClient _httpClient;

        private UserDTO? _currentUser;
        private List<MoneyTransactionDTO>? _transactionsList;
        private bool _isLoading = true;
        private bool _isProcessingPayment = false;
        private string? _errorMessage = null;

        public WalletViewModel(
            IService<MoneyTransactionDTO> moneyTransactionRepository,
            AuthService authService,
            StripeService stripeService,
            NavigationService navigationService,
            HttpClient httpClient)
        {
            _moneyTransactionRepository = moneyTransactionRepository;
            _authService = authService;
            _stripeService = stripeService;
            _navigationService = navigationService;
            _httpClient = httpClient;
        }

        public UserDTO? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public List<MoneyTransactionDTO>? TransactionsList
        {
            get => _transactionsList;
            set => SetProperty(ref _transactionsList, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsProcessingPayment
        {
            get => _isProcessingPayment;
            set => SetProperty(ref _isProcessingPayment, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public override async Task InitializeAsync()
        {
            await LoadWalletDataAsync();
        }

        /// <summary>
        /// Charge toutes les données du portefeuille
        /// </summary>
        private async Task LoadWalletDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                await UpdateUserStateAsync();

                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    TransactionsList = await _moneyTransactionRepository.GetByUserAsync(jwtToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du portefeuille: {ex.Message}");
                ErrorMessage = "Erreur lors du chargement des données";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Met à jour l'état de l'utilisateur
        /// </summary>
        private async Task UpdateUserStateAsync()
        {
            if (_authService.CurrentUser == null && await _authService.IsLoggedInAsync())
            {
                await _authService.LoadCurrentUserAsync();
            }

            CurrentUser = _authService.CurrentUser;
        }

        /// <summary>
        /// Ajoute des fonds via Stripe
        /// </summary>
        public async Task AddFundsStripe(double amount)
        {
            if (IsProcessingPayment) return;

            try
            {
                IsProcessingPayment = true;
                ErrorMessage = null;

                var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(amount);
        
                if (checkoutUrl != null)
                {
                    _navigationService.NavigateTo(checkoutUrl, forceLoad: true);
                }
                else
                {
                    ErrorMessage = "Impossible de créer la session Stripe";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Stripe session: {ex.Message}");
                ErrorMessage = "Erreur lors de la création du paiement Stripe";
            }
            finally
            {
                IsProcessingPayment = false;
            }
        }

        /// <summary>
        /// Ajoute des fonds via PayPal
        /// </summary>
        public async Task AddFundsPayPal(double amount)
        {
            if (IsProcessingPayment) return;

            try
            {
                IsProcessingPayment = true;
                ErrorMessage = null;

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "Vous devez être connecté";
                    return;
                }

                // Préparer la requête
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var request = new { amount = (decimal)amount };

                // Appeler l'API pour créer l'ordre PayPal
                var response = await _httpClient.PostAsJsonAsync("paypal/create-order", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PayPalOrderResult>();
                    
                    if (!string.IsNullOrEmpty(result?.ApprovalUrl))
                    {
                        // Rediriger vers PayPal
                        _navigationService.NavigateTo(result.ApprovalUrl, forceLoad: true);
                    }
                    else
                    {
                        ErrorMessage = "Impossible de créer le paiement PayPal";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"PayPal order creation failed: {errorContent}");
                    ErrorMessage = "Erreur lors de la création du paiement PayPal";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating PayPal order: {ex.Message}");
                ErrorMessage = $"Erreur PayPal: {ex.Message}";
            }
            finally
            {
                IsProcessingPayment = false;
            }
        }
        
        /// <summary>
        /// Retire des fonds via Stripe
        /// </summary>
        public async Task WithdrawFunds(double amount)
        {
            if (amount <= 0)
            {
                ErrorMessage = "Le montant doit être positif";
                return;
            }

            if (CurrentUser?.Wallet < amount)
            {
                ErrorMessage = "Solde insuffisant";
                return;
            }

            if (IsProcessingPayment) return;

            try
            {
                IsProcessingPayment = true;
                ErrorMessage = null;

                var withdrawalUrl = await _stripeService.CreateWithdrawalSessionAsync(amount);
    
                if (withdrawalUrl != null)
                {
                    _navigationService.NavigateTo(withdrawalUrl, forceLoad: true);
                }
                else
                {
                    ErrorMessage = "Impossible de créer la session de retrait";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error withdrawing funds: {ex.Message}");
                ErrorMessage = "Erreur lors du retrait";
            }
            finally
            {
                IsProcessingPayment = false;
            }
        }

        /// <summary>
        /// Rafraîchir les données après un paiement
        /// </summary>
        public async Task RefreshDataAsync()
        {
            await LoadWalletDataAsync();
        }
        
        public async Task WithdrawFundsPayPal(double amount, string? paypalEmail)
        {
            // ... validation code ...

            if (paypalEmail == null)
            {
                _navigationService.NavigateTo("/needEmail");
            }

            var jwtToken = await _authService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await _httpClient.PostAsJsonAsync("paypal/withdraw", new 
            { 
                amount = (decimal)amount,
                paypalEmail = paypalEmail
            });

            // ... handle response ...
        }

        // DTO pour la réponse PayPal
        private class PayPalOrderResult
        {
            public string? OrderId { get; set; }
            public string? ApprovalUrl { get; set; }
            public string? Status { get; set; }
        }
    }
}