using S5_01_Blazor_CS_GOAT.Models;
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
        private string _searchTerm = string.Empty;
        private bool _isLoading = true;
        private bool _showOnlyFavorites = false;
        private int _currentPage = 1;
        private int _pageSize = 25;
        private int _totalPages = 1;
        private int _totalCount = 0;
        private int _filteredCount = 0;
        private string? _sortKey = null;
        private string _sortType = "asc";

        public HomeViewModel(IService<CaseDTO> caseRepository, AuthService authService)
        {
            _caseRepository = caseRepository;
            _authService = authService;
        }

        /// <summary>
        /// Liste des caisses affichées
        /// </summary>
        public List<CaseDTO> Cases
        {
            get => _cases;
            set => SetProperty(ref _cases, value);
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
                    CurrentPage = 1; // Reset to first page when searching
                    _ = LoadCasesAsync();
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
        /// Filtre pour afficher uniquement les favoris
        /// </summary>
        public bool ShowOnlyFavorites
        {
            get => _showOnlyFavorites;
            set
            {
                if (SetProperty(ref _showOnlyFavorites, value))
                {
                    CurrentPage = 1; // Reset to first page when filtering
                    _ = LoadCasesAsync();
                }
            }
        }

        /// <summary>
        /// Numéro de la page actuelle
        /// </summary>
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                if (SetProperty(ref _currentPage, value))
                {
                    _ = LoadCasesAsync();
                }
            }
        }

        /// <summary>
        /// Taille de la page
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (SetProperty(ref _pageSize, value))
                {
                    CurrentPage = 1; // Reset to first page when changing page size
                    _ = LoadCasesAsync();
                }
            }
        }

        /// <summary>
        /// Nombre total de pages
        /// </summary>
        public int TotalPages
        {
            get => _totalPages;
            private set => SetProperty(ref _totalPages, value);
        }

        /// <summary>
        /// Nombre total de caisses avant filtres
        /// </summary>
        public int TotalCount
        {
            get => _totalCount;
            private set => SetProperty(ref _totalCount, value);
        }

        /// <summary>
        /// Nombre de caisses après filtres
        /// </summary>
        public int FilteredCount
        {
            get => _filteredCount;
            private set => SetProperty(ref _filteredCount, value);
        }

        /// <summary>
        /// Clé de tri (propriété sur laquelle trier)
        /// </summary>
        public string? SortKey
        {
            get => _sortKey;
            set
            {
                if (SetProperty(ref _sortKey, value))
                {
                    CurrentPage = 1;
                    _ = LoadCasesAsync();
                }
            }
        }

        /// <summary>
        /// Type de tri (asc ou desc)
        /// </summary>
        public string SortType
        {
            get => _sortType;
            set
            {
                if (SetProperty(ref _sortType, value))
                {
                    CurrentPage = 1;
                    _ = LoadCasesAsync();
                }
            }
        }

        /// <summary>
        /// Dictionnaire des propriétés triables (clé = nom technique, valeur = nom affiché)
        /// </summary>
        public Dictionary<string, string> SortableProperties { get; } = new()
        {
            { "CaseName", "Nom" },
            { "CasePrice", "Prix" },
            { "Weight", "Poids" }
        };

        /// <summary>
        /// Indique si aucune caisse n'est disponible
        /// </summary>
        public bool HasNoCases => TotalCount == 0 && !IsLoading;

        /// <summary>
        /// Indique si aucune caisse ne correspond aux filtres
        /// </summary>
        public bool HasNoFilteredCases => Cases.Count == 0 && !IsLoading && (ShowOnlyFavorites || !string.IsNullOrEmpty(SearchTerm));

        /// <summary>
        /// Initialise le ViewModel et charge les caisses
        /// </summary>
        public override async Task InitializeAsync()
        {
            await LoadCasesAsync();
        }

        /// <summary>
        /// Charge les caisses avec pagination, recherche et filtres
        /// </summary>
        private async Task LoadCasesAsync()
        {
            try
            {
                IsLoading = true;
                
                // Récupérer le token JWT si l'utilisateur est connecté
                var jwtToken = await _authService.GetTokenAsync();
                
                // Construire les paramètres de requête
                var queryParams = new Dictionary<string, string>
                {
                    { "page", CurrentPage.ToString() },
                    { "pagesize", PageSize.ToString() }
                };

                // Ajouter la recherche si nécessaire
                if (!string.IsNullOrWhiteSpace(SearchTerm))
                {
                    queryParams.Add("sorttype", "search");
                    queryParams.Add("sortkey", SearchTerm);
                }
                // Ajouter le tri si une clé est sélectionnée et pas de recherche
                else if (!string.IsNullOrWhiteSpace(SortKey))
                {
                    queryParams.Add("sortkey", SortKey);
                    queryParams.Add("sorttype", SortType);
                }

                // Ajouter le filtre favoris si nécessaire
                if (ShowOnlyFavorites)
                {
                    queryParams.Add("isfavorite", "true");
                }

                // Charger les caisses avec les options
                var response = await _caseRepository.GetAllWithOptionsAsync(jwtToken, queryParams);
                
                if (response != null)
                {
                    Cases = response.Result ?? new List<CaseDTO>();
                    TotalPages = response.PageCount;
                    TotalCount = response.TotalCount;
                    FilteredCount = response.FilteredCount;
                }
                else
                {
                    Cases = new List<CaseDTO>();
                    TotalPages = 1;
                    TotalCount = 0;
                    FilteredCount = 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des caisses: {ex.Message}");
                Cases = new List<CaseDTO>();
                TotalPages = 1;
                TotalCount = 0;
                FilteredCount = 0;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
