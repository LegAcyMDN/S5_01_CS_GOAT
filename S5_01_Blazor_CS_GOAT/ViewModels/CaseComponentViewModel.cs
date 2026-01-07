using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour CaseComponent - Gère l'affichage et la navigation d'une caisse
    /// </summary>
    public class CaseComponentViewModel : ViewModelBase
    {
        private readonly NavigationManager _navigation;
        private readonly FavoriteService _favoriteService;
        private readonly AuthService _authService;
        private Case? _caseObject;
        private bool _isAuthenticated;

        public CaseComponentViewModel(NavigationManager navigation, FavoriteService favoriteService, AuthService authService)
        {
            _navigation = navigation;
            _favoriteService = favoriteService;
            _authService = authService;
        }

        public Case? CaseObject
        {
            get => _caseObject;
            set => SetProperty(ref _caseObject, value);
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        /// <summary>
        /// Navigue vers la page de détail de la caisse
        /// </summary>
        public void NavigateToCase()
        {
            if (CaseObject != null)
            {
                _navigation.NavigateTo($"/caseview/{CaseObject.CaseId}");
            }
        }

        /// <summary>
        /// Bascule le statut favori de la caisse
        /// </summary>
        public async Task ToggleFavoriteAsync()
        {
            if (CaseObject == null || !IsAuthenticated)
                return;

            bool success = await _favoriteService.ToggleFavoriteAsync(CaseObject.CaseId, CaseObject.IsFavorite);
            
            if (success)
            {
                CaseObject.IsFavorite = !CaseObject.IsFavorite;
                OnPropertyChanged(nameof(CaseObject));
            }
        }

        /// <summary>
        /// Initialise le statut d'authentification
        /// </summary>
        public async Task InitializeAsync()
        {
            IsAuthenticated = await _authService.IsAuthenticatedAsync();
        }
    }
}
