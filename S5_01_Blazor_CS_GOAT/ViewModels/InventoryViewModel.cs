using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using Microsoft.AspNetCore.Components;
using S5_01_Blazor_CS_GOAT.Models;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Inventory - Refactorisé selon le principe SRP
    /// Responsabilité : Afficher et gérer l'inventaire utilisateur avec pagination, tri et filtrage
    /// </summary>
    public class InventoryViewModel : ViewModelBase
    {
        private readonly IService<InventoryItemDetailDTO> _inventoryService;
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;

        private List<InventoryItemDetailDTO>? _inventoryItems;
        private bool _isLoading = true;
        private bool _isAuthenticated;
        
        // Pagination
        private int _currentPage = 1;
        private int _pageSize = 50;
        private int _totalPages = 1;
        private int _totalCount = 0;
        private int _filteredCount = 0;
        
        // Tri et filtrage
        private string? _sortKey;
        private string _sortType = "desc";
        private bool _showOnlyFavorites = false;
        private Dictionary<string, string> _filters = new();

        public InventoryViewModel(
            IService<InventoryItemDetailDTO> inventoryService,
            AuthService authService,
            NavigationService navigationService)
        {
            _inventoryService = inventoryService;
            _authService = authService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Liste des items de l'inventaire
        /// </summary>
        public List<InventoryItemDetailDTO>? InventoryItems
        {
            get => _inventoryItems;
            set => SetProperty(ref _inventoryItems, value);
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
        /// Indique si l'utilisateur est authentifié
        /// </summary>
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        /// <summary>
        /// Nombre total d'items dans l'inventaire
        /// </summary>
        public int TotalItemsCount => _totalCount;

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

        // Propriétés de tri et filtrage
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

        public bool ShowOnlyFavorites
        {
            get => _showOnlyFavorites;
            set => SetProperty(ref _showOnlyFavorites, value);
        }

        /// <summary>
        /// Propriétés triables pour le filtre
        /// </summary>
        public Dictionary<string, string> SortableProperties { get; } = new()
        {
            { "AcquiredOn", "Date d'acquisition" },
            { "RarityColor", "Rareté" }
        };

        /// <summary>
        /// Vérifie si l'inventaire est vide
        /// </summary>
        public bool IsInventoryEmpty => InventoryItems == null || !InventoryItems.Any();

        /// <summary>
        /// Initialise le ViewModel et charge les données
        /// </summary>
        public override async Task InitializeAsync()
        {
            await LoadInventoryAsync();
        }

        /// <summary>
        /// Charge l'inventaire de l'utilisateur avec pagination et filtrage
        /// </summary>
        private async Task LoadInventoryAsync()
        {
            try
            {
                IsLoading = true;
                IsAuthenticated = await _authService.IsAuthenticatedAsync();

                if (IsAuthenticated)
                {
                    var token = await _authService.GetTokenAsync();
                    if (!string.IsNullOrEmpty(token))
                    {
                        var queryParams = BuildQueryParams();
                        
                        GetOptionsResponse<InventoryItemDetailDTO>? response;
                        
                        // Utiliser GetByUserWithOptionsAsync pour récupérer l'inventaire de l'utilisateur connecté
                        response = await _inventoryService.GetByUserWithOptionsAsync(token, queryParams);

                        if (response != null)
                        {
                            InventoryItems = response.Result;
                            TotalCount = response.TotalCount;
                            FilteredCount = response.FilteredCount;
                            TotalPages = response.PageCount;
                            CurrentPage = response.PageNumber;
                            PageSize = response.PageSize;
                        }
                        else
                        {
                            InventoryItems = new List<InventoryItemDetailDTO>();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'inventaire: {ex.Message}");
                InventoryItems = new List<InventoryItemDetailDTO>();
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Construit les paramètres de requête pour GetOptions
        /// </summary>
        private Dictionary<string, string> BuildQueryParams()
        {
            var queryParams = new Dictionary<string, string>
            {
                { "page", CurrentPage.ToString() },
                { "pagesize", PageSize.ToString() }
            };

            if (!string.IsNullOrEmpty(SortKey))
            {
                queryParams["sortkey"] = SortKey;
                queryParams["sorttype"] = SortType;
            }

            if (ShowOnlyFavorites)
            {
                queryParams["isfavorite"] = "true";
            }

            // Ajouter les autres filtres
            foreach (var filter in _filters)
            {
                queryParams[filter.Key] = filter.Value;
            }

            return queryParams;
        }

        /// <summary>
        /// Change la page actuelle
        /// </summary>
        public async Task ChangePageAsync(int newPage)
        {
            if (newPage != CurrentPage && newPage >= 1 && newPage <= TotalPages)
            {
                CurrentPage = newPage;
                await LoadInventoryAsync();
            }
        }

        /// <summary>
        /// Change la taille de la page
        /// </summary>
        public async Task ChangePageSizeAsync(int newPageSize)
        {
            if (newPageSize != PageSize && newPageSize > 0)
            {
                PageSize = newPageSize;
                CurrentPage = 1; // Retour à la première page
                await LoadInventoryAsync();
            }
        }

        /// <summary>
        /// Change la clé de tri
        /// </summary>
        public async Task ChangeSortKeyAsync(string? newSortKey)
        {
            if (SortKey != newSortKey)
            {
                SortKey = newSortKey;
                CurrentPage = 1;
                await LoadInventoryAsync();
            }
        }

        /// <summary>
        /// Change le type de tri
        /// </summary>
        public async Task ChangeSortTypeAsync(string newSortType)
        {
            if (SortType != newSortType)
            {
                SortType = newSortType;
                CurrentPage = 1;
                await LoadInventoryAsync();
            }
        }

        /// <summary>
        /// Bascule le filtre favoris
        /// </summary>
        public async Task ToggleFavoritesFilterAsync(bool showFavorites)
        {
            if (ShowOnlyFavorites != showFavorites)
            {
                ShowOnlyFavorites = showFavorites;
                CurrentPage = 1;
                await LoadInventoryAsync();
            }
        }

        /// <summary>
        /// Applique ou retire le filtre par couleur de rareté
        /// </summary>
        public async Task ToggleRarityColorFilterAsync(string rarityColor)
        {
            // Si le filtre existe déjà, on le retire, sinon on l'ajoute
            if (_filters.ContainsKey("raritycolor") && _filters["raritycolor"] == rarityColor)
            {
                _filters.Remove("raritycolor");
            }
            else
            {
                _filters["raritycolor"] = rarityColor;
            }
            
            CurrentPage = 1;
            await LoadInventoryAsync();
        }

        /// <summary>
        /// Obtient la couleur de rareté actuellement filtrée (si existe)
        /// </summary>
        public string? CurrentRarityColorFilter => 
            _filters.ContainsKey("raritycolor") ? _filters["raritycolor"] : null;

        /// <summary>
        /// Retourne les items de l'inventaire (déjà paginés et triés par le backend)
        /// </summary>
        public IEnumerable<InventoryItemDetailDTO> GetInventoryItems()
        {
            return InventoryItems ?? Enumerable.Empty<InventoryItemDetailDTO>();
        }

        /// <summary>
        /// Retourne les items triés par date d'acquisition
        /// </summary>
        public IEnumerable<InventoryItemDetailDTO> GetSortedInventoryItems()
        {
            if (InventoryItems == null)
                return Enumerable.Empty<InventoryItemDetailDTO>();

            return InventoryItems.OrderByDescending(item => item.AcquiredOn);
        }

        /// <summary>
        /// Navigation vers les détails d'un item
        /// </summary>
        public void NavigateToItemDetail(int inventoryItemId)
        {
            _navigationService.NavigateToItemDetail(inventoryItemId);
        }

        /// <summary>
        /// Obtient l'URL de l'image d'un item
        /// </summary>
        public string GetItemImageUrl(InventoryItemDetailDTO item)
        {
            if (!string.IsNullOrEmpty(item.Uuid))
            {
                return $"https://screenshots.cs.money/csmoney2/{item.Uuid}_icon.png";
            }
            return "images/default-item.png";
        }

        /// <summary>
        /// Bascule le statut favori d'un item
        /// </summary>
        public async Task ToggleFavoriteAsync(int inventoryItemId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    var item = InventoryItems?.FirstOrDefault(i => i.InventoryItemId == inventoryItemId);
                    if (item != null)
                    {
                        item.IsFavorite = !item.IsFavorite;
                        OnPropertyChanged(nameof(InventoryItems));

                        await _inventoryService.ToggleFavoriteAsync(inventoryItemId, token);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du toggle favori: {ex.Message}");
            }
        }
    }
}
