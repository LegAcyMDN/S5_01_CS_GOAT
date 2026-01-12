using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Home - Gère l'affichage et le filtrage des caisses
    /// </summary>
    public class HomeViewModel : ViewModelBase
    {
        private readonly IService<CaseDTO> _caseRepository;
        private readonly AuthService _authService;

        private List<CaseDTO> _cases = new();
        private List<CaseDTO> _filteredCases = new();
        private string _searchTerm = string.Empty;
        private bool _isLoading = true;

        public HomeViewModel(IService<CaseDTO> caseRepository, AuthService authService)
        {
            _caseRepository = caseRepository;
            _authService = authService;
        }

        /// <summary>
        /// Liste complète des caisses
        /// </summary>
        public List<CaseDTO> Cases
        {
            get => _cases;
            set
            {
                if (SetProperty(ref _cases, value))
                {
                    FilterCases();
                }
            }
        }

        /// <summary>
        /// Liste des caisses filtrées selon le terme de recherche
        /// </summary>
        public List<CaseDTO> FilteredCases
        {
            get => _filteredCases;
            private set => SetProperty(ref _filteredCases, value);
        }

        /// <summary>
        /// Terme de recherche pour filtrer les caisses
        /// </summary>
        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (SetProperty(ref _searchTerm, value))
                {
                    FilterCases();
                }
            }
        }

        /// <summary>
        /// Indique si les données sont en cours de chargement
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Indique si aucune caisse n'est disponible
        /// </summary>
        public bool HasNoCases => Cases.Count == 0;

        /// <summary>
        /// Indique si aucune caisse ne correspond à la recherche
        /// </summary>
        public bool HasNoFilteredCases => FilteredCases.Count == 0 && !string.IsNullOrEmpty(SearchTerm);

        /// <summary>
        /// Initialise le ViewModel et charge les caisses
        /// </summary>
        public override async Task InitializeAsync()
        {
            await LoadCasesAsync();
        }

        /// <summary>
        /// Charge toutes les caisses disponibles
        /// </summary>
        private async Task LoadCasesAsync()
        {
            try
            {
                IsLoading = true;
                
                // Récupérer le token JWT si l'utilisateur est connecté
                var jwtToken = await _authService.GetTokenAsync();
                
                // Charger les caisses avec le token pour obtenir l'état IsFavorite correct
                Cases = await _caseRepository.GetAllAsync(jwtToken) ?? new List<CaseDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des caisses: {ex.Message}");
                Cases = new List<CaseDTO>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Filtre les caisses selon le terme de recherche
        /// </summary>
        private void FilterCases()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                FilteredCases = Cases;
            }
            /*else
            {
                FilteredCases = Cases
                    .Where(c => c.Name != null && c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }*/
        }
    }
}
