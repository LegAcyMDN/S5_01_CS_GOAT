using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour AuthOverlay - Gère l'affichage de l'overlay d'authentification
    /// </summary>
    public class AuthOverlayViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationManager _navigation;
        private bool _isAuthenticated = false;

        public AuthOverlayViewModel(AuthService authService, NavigationManager navigation)
        {
            _authService = authService;
            _navigation = navigation;
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        public bool ShowOverlay => !IsAuthenticated;

        public override async Task InitializeAsync()
        {
            IsAuthenticated = await _authService.IsLoggedInAsync();
        }

        /// <summary>
        /// Navigue vers la page de connexion
        /// </summary>
        public void NavigateToLogin()
        {
            _navigation.NavigateTo("/login");
        }

        /// <summary>
        /// Navigue vers la page d'inscription
        /// </summary>
        public void NavigateToRegister()
        {
            _navigation.NavigateTo("/register");
        }
    }
}
