using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour AuthOverlay - Refactorisé selon le principe SRP
    /// Responsabilité : Gérer l'affichage de l'overlay d'authentification
    /// </summary>
    public class AuthOverlayViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;
        private bool _isAuthenticated = false;

        public AuthOverlayViewModel(AuthService authService, NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
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

        public void NavigateToLogin() => _navigationService.NavigateToLogin();

        public void NavigateToRegister() => _navigationService.NavigateToRegister();
    }
}
