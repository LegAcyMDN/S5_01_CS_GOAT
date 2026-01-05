using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Wallet - Gère le portefeuille et les limites
    /// </summary>
    public class WalletViewModel : ViewModelBase
    {
        private readonly IService<MoneyTransaction> _moneyTransactionRepository;
        private readonly IService<Limit> _limitRepository;
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly NavigationManager  _navigationManager;
        private readonly StripeService _stripeService;

        private User? _currentUser;
        private List<MoneyTransaction>? _transactionsList;
        private List<Limit>? _limits;
        private string _type = "Aucune";
        private string _period = "Aucune";
        private double? _amount = null;
        private bool _isLoading = true;

        public WalletViewModel(
            IService<MoneyTransaction> moneyTransactionRepository,
            IService<Limit> limitRepository,
            AuthService authService,
            HttpClient httpClient, 
            NavigationManager navigationManager, 
            StripeService stripeService)
        {
            _moneyTransactionRepository = moneyTransactionRepository;
            _limitRepository = limitRepository;
            _authService = authService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _stripeService = stripeService;
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public List<MoneyTransaction>? TransactionsList
        {
            get => _transactionsList;
            set => SetProperty(ref _transactionsList, value);
        }

        public List<Limit>? Limits
        {
            get => _limits;
            set => SetProperty(ref _limits, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        public double? Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
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

                await UpdateUserStateAsync();

                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    TransactionsList = await _moneyTransactionRepository.GetByUserAsync(jwtToken);
                    Limits = await _limitRepository.GetByUserAsync(jwtToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du portefeuille: {ex.Message}");
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
        /// Sauvegarde une limite de budget
        /// </summary>
        public async Task SaveLimitAsync()
        {
            if (Limits == null)
            {
                Console.WriteLine("Aucune limite chargée.");
                return;
            }

            if (string.IsNullOrEmpty(Type) || Type == "Aucune")
            {
                Console.WriteLine("Sélectionne un type avant de sauvegarder.");
                return;
            }

            if (string.IsNullOrEmpty(Period) || Period == "Aucune")
            {
                Console.WriteLine("Sélectionne une période avant de sauvegarder.");
                return;
            }

            var limitTypeName = $"{Type} {Period}";

            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Utilisateur non authentifié.");
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var payload = new { LimitAmount = Amount, LimitTypeName = limitTypeName };
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"Limit/update")
            {
                Content = JsonContent.Create(payload)
            };

            var resp = await _httpClient.SendAsync(request);
            if (resp.IsSuccessStatusCode)
            {
                var jwtToken = await _authService.GetTokenAsync();
                Limits = await _limitRepository.GetByUserAsync(jwtToken);
            }
            else
            {
                var err = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur sauvegarde limite: {err}");
            }
        }
        public async Task AddFunds(double amount)
        {
            var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(amount);
        
            if (checkoutUrl != null)
            {
                // Redirect to Stripe checkout
                _navigationManager.NavigateTo(checkoutUrl, forceLoad: true);
            }
            else
            {
                Console.WriteLine("Failed to create checkout session");
            }
        }
        
        public async Task WithdrawFunds(double amount)
        {
            if (amount <= 0)
            {
                Console.WriteLine("Amount must be positive");
                return;
            }

            if (CurrentUser?.Wallet < amount)
            {
                Console.WriteLine("Insufficient funds");
                return;
            }

            try
            {
                var withdrawalUrl = await _stripeService.CreateWithdrawalSessionAsync(amount);
    
                if (withdrawalUrl != null)
                {
                    _navigationManager.NavigateTo(withdrawalUrl, forceLoad: true);
                }
                else
                {
                    Console.WriteLine("Failed to create withdrawal session");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error withdrawing funds: {ex.Message}");
            }
        }
    }
    
}
