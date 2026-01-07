using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page FairRandom - Gère l'historique des tirages provably fair
    /// </summary>
    public class FairRandomViewModel : ViewModelBase
    {
        private readonly IService<FairRandomDTO> _fairRandomService;
        private readonly AuthService _authService;
        private List<FairRandomDTO> _fairRandomHistory = new();
        private bool _isLoading = true;
        private bool _isAuthenticated = false;
        private string? _errorMessage;

        public FairRandomViewModel(IService<FairRandomDTO> fairRandomService, AuthService authService)
        {
            _fairRandomService = fairRandomService;
            _authService = authService;
        }

        public List<FairRandomDTO> FairRandomHistory
        {
            get => _fairRandomHistory;
            set => SetProperty(ref _fairRandomHistory, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Charge l'historique des tirages provably fair de l'utilisateur
        /// </summary>
        public override async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;
                
                IsAuthenticated = await _authService.IsAuthenticatedAsync();
                
                if (!IsAuthenticated)
                {
                    ErrorMessage = "Vous devez être connecté pour voir l'historique des tirages.";
                    return;
                }

                var jwtToken = await _authService.GetTokenAsync();
                var history = await _fairRandomService.GetByUserAsync(jwtToken);
                
                FairRandomHistory = history ?? new List<FairRandomDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'historique: {ex.Message}");
                ErrorMessage = "Une erreur est survenue lors du chargement de l'historique.";
                FairRandomHistory = new List<FairRandomDTO>();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
