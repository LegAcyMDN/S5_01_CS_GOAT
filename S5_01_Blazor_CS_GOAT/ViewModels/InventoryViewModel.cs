using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Inventory - Refactorisé selon le principe SRP
    /// Responsabilité : Afficher et gérer l'inventaire utilisateur
    /// </summary>
    public class InventoryViewModel : ViewModelBase
    {
        private readonly IService<InventoryItemDetailDTO> _inventoryService;
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;

        private List<InventoryItemDetailDTO>? _inventoryItems;
        private bool _isLoading = true;
        private bool _isAuthenticated;

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
        public int TotalItemsCount => InventoryItems?.Count ?? 0;

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
        /// Charge l'inventaire de l'utilisateur
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
                        InventoryItems = await _inventoryService.GetByUserAsync(token);
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
