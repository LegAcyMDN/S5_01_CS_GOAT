using S5_01_Blazor_CS_GOAT.Models;
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
        
        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 25;
        private int _totalPages = 1;
        private int _totalCount = 0;
        private int _filteredCount = 0;
        
        // Recherche et tri
        private string _searchTerm = string.Empty;
        private string? _sortKey = "UserNonce";
        private string _sortType = "desc";

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

        // Propriétés de pagination
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public int FilteredCount
        {
            get => _filteredCount;
            set => SetProperty(ref _filteredCount, value);
        }

        // Propriétés de recherche et tri
        public string SearchTerm
        {
            get => _searchTerm;
            set => SetProperty(ref _searchTerm, value);
        }

        public string? SortKey
        {
            get => _sortKey;
            set => SetProperty(ref _sortKey, value);
        }

        public string SortType
        {
            get => _sortType;
            set => SetProperty(ref _sortType, value);
        }

        public Dictionary<string, string> SortableProperties => new()
        {
            { "UserNonce", "Nonce Utilisateur" },
            { "ServerSeed", "Graine Serveur" },
            { "ServerHash", "Hash Serveur" },
            { "UserSeed", "Graine Utilisateur" },
            { "CombinedHash", "Hash Combiné" },
            { "Fraction1", "Fraction 1" },
            { "Fraction2", "Fraction 2" }
        };

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

                await LoadHistoryAsync();
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

        /// <summary>
        /// Charge l'historique avec les options de pagination et filtrage
        /// </summary>
        public async Task LoadHistoryAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = null;

                var jwtToken = await _authService.GetTokenAsync();
                var queryParams = BuildQueryParams();

                var response = await _fairRandomService.GetByUserWithOptionsAsync(jwtToken, queryParams);

                if (response != null)
                {
                    FairRandomHistory = response.Result ?? new List<FairRandomDTO>();
                    CurrentPage = response.PageNumber;
                    PageSize = response.PageSize;
                    TotalPages = response.PageCount;
                    TotalCount = response.TotalCount;
                    FilteredCount = response.FilteredCount;
                }
                else
                {
                    FairRandomHistory = new List<FairRandomDTO>();
                }
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

        /// <summary>
        /// Construit les paramètres de requête pour l'API
        /// </summary>
        private Dictionary<string, string> BuildQueryParams()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pagesize", PageSize.ToString() }
            };

            // Recherche
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                queryParams["sorttype"] = "search";
                queryParams["sortkey"] = SearchTerm;
            }
            // Tri normal
            else if (!string.IsNullOrEmpty(SortKey))
            {
                queryParams["sortkey"] = SortKey;
                queryParams["sorttype"] = SortType;
            }

            return queryParams;
        }

        /// <summary>
        /// Change la page actuelle
        /// </summary>
        public async Task OnPageChangedAsync(int newPage)
        {
            if (newPage != CurrentPage && newPage >= 1 && newPage <= TotalPages)
            {
                CurrentPage = newPage;
                await LoadHistoryAsync();
            }
        }

        /// <summary>
        /// Change la taille de page
        /// </summary>
        public async Task OnPageSizeChangedAsync(int newSize)
        {
            if (newSize != PageSize && newSize > 0)
            {
                PageSize = newSize;
                CurrentPage = 1; // Reset à la première page
                await LoadHistoryAsync();
            }
        }

        /// <summary>
        /// Change le terme de recherche
        /// </summary>
        public async Task OnSearchTermChangedAsync(string newSearchTerm)
        {
            if (newSearchTerm != SearchTerm)
            {
                SearchTerm = newSearchTerm;
                CurrentPage = 1; // Reset à la première page
                await LoadHistoryAsync();
            }
        }

        /// <summary>
        /// Change la clé de tri
        /// </summary>
        public async Task OnSortKeyChangedAsync(string? newSortKey)
        {
            if (newSortKey != SortKey)
            {
                SortKey = newSortKey;
                CurrentPage = 1; // Reset à la première page
                await LoadHistoryAsync();
            }
        }

        /// <summary>
        /// Change le type de tri
        /// </summary>
        public async Task OnSortTypeChangedAsync(string newSortType)
        {
            if (newSortType != SortType)
            {
                SortType = newSortType;
                CurrentPage = 1; // Reset à la première page
                await LoadHistoryAsync();
            }
        }
    }
}
