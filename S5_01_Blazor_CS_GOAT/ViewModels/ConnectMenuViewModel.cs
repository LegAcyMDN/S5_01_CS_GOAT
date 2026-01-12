using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour ConnectMenu - Refactorisé selon le principe SRP
    /// Responsabilité : Gérer l'affichage du menu utilisateur
    /// </summary>
    public class ConnectMenuViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;
        private UserDTO? _currentUser;

        public ConnectMenuViewModel(AuthService authService, NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        
            // Subscribe to AuthService changes
            _authService.UserDataChanged += OnUserDataChanged;
        }

        public UserDTO? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool IsAuthenticated => CurrentUser != null;

        public override async Task InitializeAsync()
        {
            await UpdateUserStateAsync();
        }

        private void OnUserDataChanged(object? sender, EventArgs e)
        {
            // Update immediately when AuthService changes
            CurrentUser = _authService.CurrentUser;
            Console.WriteLine($"🔄 Menu updated: Wallet = {CurrentUser?.Wallet}");
        }

        public async Task UpdateUserStateAsync()
        {
            if (_authService.CurrentUser == null && await _authService.IsLoggedInAsync())
            {
                await _authService.LoadCurrentUserAsync();
            }

            CurrentUser = _authService.CurrentUser;
        }

        public void NavigateToLogin() => _navigationService.NavigateToLogin();

        public void NavigateToWallet() => _navigationService.NavigateToWallet();

        public void NavigateToProfile() => _navigationService.NavigateToProfile();

        public void NavigateToAdmin() => _navigationService.NavigateToAdmin();

        public async Task HandleLogoutAsync()
        {
            await _authService.LogoutAsync();
            CurrentUser = null;
            _navigationService.NavigateToHome(forceLoad: true);
        }

        public void Dispose()
        {
            // Unsubscribe when disposed
            _authService.UserDataChanged -= OnUserDataChanged;
        }
    }
}
