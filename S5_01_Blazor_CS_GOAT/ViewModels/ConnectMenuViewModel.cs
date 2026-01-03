using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour ConnectMenu - Gère l'état de connexion et le menu utilisateur
    /// </summary>
    public class ConnectMenuViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationManager _navigation;
        private User? _currentUser;

        public ConnectMenuViewModel(AuthService authService, NavigationManager navigation)
        {
            _authService = authService;
            _navigation = navigation;
        
            // Subscribe to AuthService changes
            _authService.UserDataChanged += OnUserDataChanged;
        }

        public User? CurrentUser
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

        public void NavigateToLogin()
        {
            _navigation.NavigateTo("/login");
        }

        public void NavigateToWallet()
        {
            _navigation.NavigateTo("/wallet");
        }

        public void NavigateToProfile()
        {
            _navigation.NavigateTo("/profile");
        }

        public void NavigateToAdmin()
        {
            _navigation.NavigateTo("/admin");
        }

        public async Task HandleLogoutAsync()
        {
            await _authService.LogoutAsync();
            CurrentUser = null;
            _navigation.NavigateTo("/", forceLoad: true);
        }

        public void Dispose()
        {
            // Unsubscribe when disposed
            _authService.UserDataChanged -= OnUserDataChanged;
        }
    }
}
