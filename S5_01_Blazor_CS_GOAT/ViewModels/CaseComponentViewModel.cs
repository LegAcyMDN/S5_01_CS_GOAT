using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour CaseComponent - Refactorisé selon le principe SRP
    /// Responsabilité : Afficher un composant de caisse
    /// </summary>
    public class CaseComponentViewModel : ViewModelBase
    {
        private readonly NavigationService _navigationService;
        private readonly FavoriteService _favoriteService;
        private readonly AuthService _authService;
        private CaseDTO? _caseObject;
        private bool _isAuthenticated;

        public CaseComponentViewModel(
            NavigationService navigationService,
            FavoriteService favoriteService,
            AuthService authService)
        {
            _navigationService = navigationService;
            _favoriteService = favoriteService;
            _authService = authService;
        }

        public CaseDTO? CaseObject
        {
            get => _caseObject;
            set => SetProperty(ref _caseObject, value);
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        public void NavigateToCase()
        {
            if (CaseObject != null)
            {
                _navigationService.NavigateToCase(CaseObject.CaseId);
            }
        }

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

        public override async Task InitializeAsync()
        {
            IsAuthenticated = await _authService.IsAuthenticatedAsync();
        }
    }
}
