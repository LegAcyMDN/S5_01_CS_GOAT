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
        private System.Threading.Timer? _timer;

        public ConnectMenuViewModel(AuthService authService, NavigationManager navigation)
        {
            _authService = authService;
            _navigation = navigation;
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
            StartPeriodicUpdate();
        }

        /// <summary>
        /// Met à jour l'état de l'utilisateur
        /// </summary>
        public async Task UpdateUserStateAsync()
        {
            if (_authService.CurrentUser == null && await _authService.IsLoggedInAsync())
            {
                await _authService.LoadCurrentUserAsync();
            }

            CurrentUser = _authService.CurrentUser;
        }

        /// <summary>
        /// Démarre la mise à jour périodique de l'état de connexion
        /// </summary>
        private void StartPeriodicUpdate()
        {
            _timer = new System.Threading.Timer(async _ =>
            {
                await UpdateUserStateAsync();
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Navigation vers la page de login
        /// </summary>
        public void NavigateToLogin()
        {
            _navigation.NavigateTo("/login");
        }

        /// <summary>
        /// Navigation vers la page du portefeuille
        /// </summary>
        public void NavigateToWallet()
        {
            _navigation.NavigateTo("/wallet");
        }

        /// <summary>
        /// Navigation vers la page du profil
        /// </summary>
        public void NavigateToProfile()
        {
            _navigation.NavigateTo("/profile");
        }

        /// <summary>
        /// Navigation vers le panneau d'administration
        /// </summary>
        public void NavigateToAdmin()
        {
            _navigation.NavigateTo("/admin");
        }

        /// <summary>
        /// Déconnexion de l'utilisateur
        /// </summary>
        public async Task HandleLogoutAsync()
        {
            await _authService.LogoutAsync();
            CurrentUser = null;
            _navigation.NavigateTo("/", forceLoad: true);
        }

        /// <summary>
        /// Dispose des ressources
        /// </summary>
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
