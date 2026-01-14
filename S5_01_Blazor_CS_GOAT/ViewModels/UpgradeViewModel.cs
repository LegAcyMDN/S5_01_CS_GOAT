using S5_01_Blazor_CS_GOAT.Service;
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
        private bool _showFavoritesOnly = false;

        public UpgradeViewModel(
            IService<SkinDTO> skinService,
            IService<InventoryItemDTO> inventoryService,
            AuthService authService,
            UpgradeService upgradeService)
        {
            _skinService = skinService;
            _inventoryService = inventoryService;
            _authService = authService;
            _upgradeService = upgradeService;
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

        public bool ShowFavoritesOnly
        {
            get => _showFavoritesOnly;
            set
            {
                if (SetProperty(ref _showFavoritesOnly, value))
                {
                    _ = LoadSkinsAsync();
                }
            }
        }

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
                if (string.IsNullOrEmpty(token))
                    return;

                var queryParams = new Dictionary<string, string>
                {
                    { "pagesize", "500" },
                    { "sortkey", "Weight" },
                    { "sorttype", "desc" }
                };

                var response = ShowFavoritesOnly 
                    ? await _skinService.GetAllWithFavoriteFilterAsync(token, queryParams)
                    : await _skinService.GetAllWithOptionsAsync(token, queryParams);

                AvailableSkins = response?.Result?.OrderByDescending(s => s.Weight ?? 0).ToList();
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
                    return;

                UserInventory = await _inventoryService.GetByUserAsync(token);
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
