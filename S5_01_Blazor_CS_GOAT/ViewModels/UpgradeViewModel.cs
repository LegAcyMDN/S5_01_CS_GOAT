using S5_01_Blazor_CS_GOAT.Service;
using S5_01_Blazor_CS_GOAT.Models;
using Shared.DTO;
using Shared.DTO.Helpers;
using System.Collections.ObjectModel;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Upgrade - Gère l'amélioration des skins
    /// </summary>
    public class UpgradeViewModel : ViewModelBase
    {
        private readonly IService<SkinDTO> _skinService;
        private readonly IService<InventoryItemDTO> _inventoryService;
        private readonly AuthService _authService;
        private readonly UpgradeService _upgradeService;
        private readonly GetOptionsService _getOptionsService;

        private bool _isLoading = true;
        private bool _isAuthenticated;
        private List<SkinDTO>? _availableSkins;
        private List<InventoryItemDTO>? _userInventory;
        private SkinDTO? _selectedTargetSkin;
        private ObservableCollection<InventoryItemDTO> _selectedItems = new();
        private UpgradeOutputDTO? _previewResult;
        private bool _isProcessing;
        private string? _errorMessage;
        private string? _successMessage;

        // Pagination et filtres pour les skins
        private string _skinSearchTerm = string.Empty;
        private string? _skinSortKey = "Weight";
        private string _skinSortType = "desc";
        private int _skinCurrentPage = 1;
        private int _skinPageSize = 50;
        private int _skinTotalPages = 1;
        private int _skinTotalCount = 0;
        private int _skinFilteredCount = 0;

        // Pagination et filtres pour l'inventaire
        private string? _inventorySortKey = "AcquiredOn";
        private string _inventorySortType = "desc";
        private int _inventoryCurrentPage = 1;
        private int _inventoryPageSize = 25;
        private int _inventoryTotalPages = 1;
        private int _inventoryTotalCount = 0;
        private int _inventoryFilteredCount = 0;

        public UpgradeViewModel(
            IService<SkinDTO> skinService,
            IService<InventoryItemDTO> inventoryService,
            AuthService authService,
            UpgradeService upgradeService,
            GetOptionsService getOptionsService)
        {
            _skinService = skinService;
            _inventoryService = inventoryService;
            _authService = authService;
            _upgradeService = upgradeService;
            _getOptionsService = getOptionsService;
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

        public List<SkinDTO>? AvailableSkins
        {
            get => _availableSkins;
            set => SetProperty(ref _availableSkins, value);
        }

        public List<InventoryItemDTO>? UserInventory
        {
            get => _userInventory;
            set => SetProperty(ref _userInventory, value);
        }

        public SkinDTO? SelectedTargetSkin
        {
            get => _selectedTargetSkin;
            set
            {
                if (SetProperty(ref _selectedTargetSkin, value))
                {
                    _ = UpdatePreviewAsync();
                }
            }
        }

        public ObservableCollection<InventoryItemDTO> SelectedItems
        {
            get => _selectedItems;
            set => SetProperty(ref _selectedItems, value);
        }

        public UpgradeOutputDTO? PreviewResult
        {
            get => _previewResult;
            set => SetProperty(ref _previewResult, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public string? ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string? SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        // Propriétés de pagination pour les skins
        public string SkinSearchTerm
        {
            get => _skinSearchTerm;
            set
            {
                if (SetProperty(ref _skinSearchTerm, value))
                {
                    SkinCurrentPage = 1;
                    _ = LoadSkinsAsync();
                }
            }
        }

        public string? SkinSortKey
        {
            get => _skinSortKey;
            set
            {
                if (SetProperty(ref _skinSortKey, value))
                {
                    _ = LoadSkinsAsync();
                }
            }
        }

        public string SkinSortType
        {
            get => _skinSortType;
            set
            {
                if (SetProperty(ref _skinSortType, value))
                {
                    _ = LoadSkinsAsync();
                }
            }
        }

        public int SkinCurrentPage
        {
            get => _skinCurrentPage;
            set
            {
                if (SetProperty(ref _skinCurrentPage, value))
                {
                    _ = LoadSkinsAsync();
                }
            }
        }

        public int SkinPageSize
        {
            get => _skinPageSize;
            set
            {
                if (SetProperty(ref _skinPageSize, value))
                {
                    SkinCurrentPage = 1;
                    _ = LoadSkinsAsync();
                }
            }
        }

        public int SkinTotalPages
        {
            get => _skinTotalPages;
            set => SetProperty(ref _skinTotalPages, value);
        }

        public int SkinTotalCount
        {
            get => _skinTotalCount;
            set => SetProperty(ref _skinTotalCount, value);
        }

        public int SkinFilteredCount
        {
            get => _skinFilteredCount;
            set => SetProperty(ref _skinFilteredCount, value);
        }

        // Propriétés de pagination pour l'inventaire
        public string? InventorySortKey
        {
            get => _inventorySortKey;
            set
            {
                if (SetProperty(ref _inventorySortKey, value))
                {
                    _ = LoadInventoryAsync();
                }
            }
        }

        public string InventorySortType
        {
            get => _inventorySortType;
            set
            {
                if (SetProperty(ref _inventorySortType, value))
                {
                    _ = LoadInventoryAsync();
                }
            }
        }

        public int InventoryCurrentPage
        {
            get => _inventoryCurrentPage;
            set
            {
                if (SetProperty(ref _inventoryCurrentPage, value))
                {
                    _ = LoadInventoryAsync();
                }
            }
        }

        public int InventoryPageSize
        {
            get => _inventoryPageSize;
            set
            {
                if (SetProperty(ref _inventoryPageSize, value))
                {
                    InventoryCurrentPage = 1;
                    _ = LoadInventoryAsync();
                }
            }
        }

        public int InventoryTotalPages
        {
            get => _inventoryTotalPages;
            set => SetProperty(ref _inventoryTotalPages, value);
        }

        public int InventoryTotalCount
        {
            get => _inventoryTotalCount;
            set => SetProperty(ref _inventoryTotalCount, value);
        }

        public int InventoryFilteredCount
        {
            get => _inventoryFilteredCount;
            set => SetProperty(ref _inventoryFilteredCount, value);
        }

        public Dictionary<string, string> SkinSortableProperties => new()
        {
            { "Weight", "Poids" },
            { "SkinName", "Nom du skin" },
            { "ItemName", "Nom de l'item" },
            { "RarityName", "Rareté" }
        };

        public Dictionary<string, string> InventorySortableProperties => new()
        {
            { "AcquiredOn", "Date d'acquisition" },
            { "RarityColor", "Rareté" }
        };

        public bool CanUpgrade => SelectedTargetSkin != null && SelectedItems.Count > 0 && !IsProcessing;

        public override async Task InitializeAsync()
        {
            IsAuthenticated = await _authService.IsAuthenticatedAsync();
            
            if (IsAuthenticated)
            {
                await Task.WhenAll(LoadSkinsAsync(), LoadInventoryAsync());
            }
            
            IsLoading = false;
        }

        private async Task LoadSkinsAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                
                var queryParams = new Dictionary<string, string>
                {
                    { "page", SkinCurrentPage.ToString() },
                    { "pagesize", SkinPageSize.ToString() }
                };

                if (!string.IsNullOrWhiteSpace(SkinSearchTerm))
                {
                    queryParams["sorttype"] = "search";
                    queryParams["sortkey"] = SkinSearchTerm;
                }
                else if (!string.IsNullOrEmpty(SkinSortKey))
                {
                    queryParams["sortkey"] = SkinSortKey;
                    queryParams["sorttype"] = SkinSortType;
                }

                var response = await _skinService.GetAllWithOptionsAsync(token, queryParams);

                if (response != null)
                {
                    AvailableSkins = response.Result;
                    SkinTotalPages = response.PageCount;
                    SkinTotalCount = response.TotalCount;
                    SkinFilteredCount = response.FilteredCount;
                }
                else
                {
                    AvailableSkins = new List<SkinDTO>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des skins: {ex.Message}";
                AvailableSkins = new List<SkinDTO>();
            }
        }

        private async Task LoadInventoryAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    UserInventory = new List<InventoryItemDTO>();
                    return;
                }

                var queryParams = new Dictionary<string, string>
                {
                    { "page", InventoryCurrentPage.ToString() },
                    { "pagesize", InventoryPageSize.ToString() }
                };

                if (!string.IsNullOrEmpty(InventorySortKey))
                {
                    queryParams["sortkey"] = InventorySortKey;
                    queryParams["sorttype"] = InventorySortType;
                }

                var response = await _inventoryService.GetByUserWithOptionsAsync(token, queryParams);

                if (response != null)
                {
                    UserInventory = response.Result;
                    InventoryTotalPages = response.PageCount;
                    InventoryTotalCount = response.TotalCount;
                    InventoryFilteredCount = response.FilteredCount;
                }
                else
                {
                    UserInventory = new List<InventoryItemDTO>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement de l'inventaire: {ex.Message}";
                UserInventory = new List<InventoryItemDTO>();
            }
        }

        public void ToggleItemSelection(InventoryItemDTO item)
        {
            if (SelectedItems.Contains(item))
            {
                SelectedItems.Remove(item);
            }
            else
            {
                SelectedItems.Add(item);
            }
            
            OnPropertyChanged(nameof(CanUpgrade));
            _ = UpdatePreviewAsync();
        }

        public bool IsItemSelected(InventoryItemDTO item)
        {
            return SelectedItems.Contains(item);
        }

        private async Task UpdatePreviewAsync()
        {
            if (!CanUpgrade)
            {
                PreviewResult = null;
                return;
            }

            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return;

                var input = new UpgradeInputDTO
                {
                    Preview = true,
                    InventoryItemIds = SelectedItems.Select(i => i.InventoryItemId).ToList(),
                    MonetaryValue = 0,
                    TargetSkinId = SelectedTargetSkin!.SkinId
                };

                PreviewResult = await _upgradeService.ExecuteUpgradeAsync(input, token);
                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'apercu: {ex.Message}";
                PreviewResult = null;
            }
        }

        public async Task ExecuteUpgradeAsync()
        {
            if (!CanUpgrade || IsProcessing)
                return;

            try
            {
                IsProcessing = true;
                ErrorMessage = null;
                SuccessMessage = null;

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return;

                var input = new UpgradeInputDTO
                {
                    Preview = false,
                    InventoryItemIds = SelectedItems.Select(i => i.InventoryItemId).ToList(),
                    MonetaryValue = 0,
                    TargetSkinId = SelectedTargetSkin!.SkinId
                };

                PreviewResult = await _upgradeService.ExecuteUpgradeAsync(input, token);
                
                if (PreviewResult != null)
                {
                    if (PreviewResult.ItemResult != null)
                    {
                        SuccessMessage = "Amelioration reussie! Vous avez obtenu un nouveau skin.";
                    }
                    else
                    {
                        SuccessMessage = "Amelioration terminee. Les objets ont ete traites.";
                    }
                    
                    SelectedItems.Clear();
                    await LoadInventoryAsync();
                    _authService.NotifyUserDataChanged();
                    
                    OnPropertyChanged(nameof(CanUpgrade));
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'amelioration: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public void ClearSelection()
        {
            SelectedItems.Clear();
            SelectedTargetSkin = null;
            PreviewResult = null;
            ErrorMessage = null;
            SuccessMessage = null;
            OnPropertyChanged(nameof(CanUpgrade));
        }

        public string GetItemImageUrl(InventoryItemDTO item)
        {
            if (!string.IsNullOrEmpty(item.Uuid))
            {
                return $"https://screenshots.cs.money/csmoney2/{item.Uuid}_icon.png";
            }
            return "images/default-item.png";
        }

        public string GetSkinImageUrl(SkinDTO skin)
        {
            if (!string.IsNullOrEmpty(skin.AnyUuid))
            {
                return $"https://screenshots.cs.money/csmoney2/{skin.AnyUuid}_icon.png";
            }
            return "images/default-item.png";
        }
    }
}
