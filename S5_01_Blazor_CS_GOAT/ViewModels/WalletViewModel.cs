using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Wallet - Refactorisé selon le principe SRP
    /// Responsabilité : Afficher le portefeuille et coordonner les opérations financières
    /// </summary>
    public class WalletViewModel : ViewModelBase
    {
        private readonly IService<MoneyTransactionDTO> _moneyTransactionRepository;
        private readonly AuthService _authService;
        private readonly StripeService _stripeService;
        private readonly NavigationService _navigationService;

        private UserDTO? _currentUser;
        private List<MoneyTransactionDTO>? _transactionsList;
        private bool _isLoading = true;

        public WalletViewModel(
            IService<MoneyTransactionDTO> moneyTransactionRepository,
            AuthService authService,
            StripeService stripeService,
            NavigationService navigationService)
        {
            _moneyTransactionRepository = moneyTransactionRepository;
            _authService = authService;
            _stripeService = stripeService;
            _navigationService = navigationService;
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
        /// Ajoute des fonds via Stripe
        /// </summary>
        public async Task AddFunds(double amount)
        {
            var checkoutUrl = await _stripeService.CreateCheckoutSessionAsync(amount);
        
            if (checkoutUrl != null)
            {
                _navigationService.NavigateTo(checkoutUrl, forceLoad: true);
            }
            else
            {
                Console.WriteLine("Failed to create checkout session");
            }
        }
        
        /// <summary>
        /// Retire des fonds via Stripe
        /// </summary>
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
                    _navigationService.NavigateTo(withdrawalUrl, forceLoad: true);
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
