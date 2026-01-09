using HomagGroup.Blazor3D.Enums;
using HomagGroup.Blazor3D.Events;
using HomagGroup.Blazor3D.Lights;
using HomagGroup.Blazor3D.Maths;
using HomagGroup.Blazor3D.Scenes;
using HomagGroup.Blazor3D.Settings;
using HomagGroup.Blazor3D.Viewers;
using Microsoft.JSInterop;
using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using System.Collections.ObjectModel;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page 3DView - Gère le chargement et l'affichage des modèles 3D
    /// </summary>
    public class ThreeDViewViewModel : ViewModelBase
    {
        private readonly IThreeDModelService<ThreeDModel> _threeDModelRepository;
        private readonly IService<InventoryItemDetail> _inventoryItemService;
        private readonly IService<PriceHistoryDTO> _priceHistoryService;
        private readonly AuthService _authService;
        private readonly CacheService _cacheService;
        private readonly IJSRuntime _jsRuntime;

        private InventoryItemDetail? _itemDetails;
        private string? _modelUrl;
        private bool _isLoading = true;
        private string _loadingMessage = string.Empty;
        private int _loadingProgress = 0;
        private Guid _loadedObjectGuid = Guid.NewGuid();
        private Scene _scene = new Scene();
        private ObservableCollection<PriceHistoryDTO>? _priceHistory = new ObservableCollection<PriceHistoryDTO>();
        private bool _isLoadingPriceHistory;

        public ThreeDViewViewModel(
            IThreeDModelService<ThreeDModel> threeDModelRepository,
            IService<InventoryItemDetail> inventoryItemService,
            IService<PriceHistoryDTO> priceHistoryService,
            AuthService authService,
            CacheService cacheService,
            IJSRuntime jsRuntime)
        {
            _threeDModelRepository = threeDModelRepository;
            _inventoryItemService = inventoryItemService;
            _priceHistoryService = priceHistoryService;
            _authService = authService;
            _cacheService = cacheService;
            _jsRuntime = jsRuntime;
        }

        public InventoryItemDetail? ItemDetails
        {
            get => _itemDetails;
            set => SetProperty(ref _itemDetails, value);
        }

        public string? ModelUrl
        {
            get => _modelUrl;
            set => SetProperty(ref _modelUrl, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string LoadingMessage
        {
            get => _loadingMessage;
            set => SetProperty(ref _loadingMessage, value);
        }

        public int LoadingProgress
        {
            get => _loadingProgress;
            set => SetProperty(ref _loadingProgress, value);
        }

        public Scene Scene
        {
            get => _scene;
            set => SetProperty(ref _scene, value);
        }

        public Guid LoadedObjectGuid
        {
            get => _loadedObjectGuid;
            set => SetProperty(ref _loadedObjectGuid, value);
        }

        public ObservableCollection<PriceHistoryDTO>? PriceHistory
        {
            get => _priceHistory;
            set => SetProperty(ref _priceHistory, value);
        }

        public bool IsLoadingPriceHistory
        {
            get => _isLoadingPriceHistory;
            set => SetProperty(ref _isLoadingPriceHistory, value);
        }

        /// <summary>
        /// Initialise les lumières de la scène 3D
        /// </summary>
        public void InitializeLights()
        {
            Scene.Add(new AmbientLight());
            Scene.Add(new PointLight()
            {
                Intensity = 0.5f,
                Position = new Vector3(100, 200, 100)
            });
            Scene.Add(new PointLight()
            {
                Intensity = 1f,
                Position = new Vector3(5, 5, 5)
            });
        }

        /// <summary>
        /// Charge le modèle 3D pour un item d'inventaire
        /// </summary>
        public async Task<Guid> LoadModelAsync(int inventoryItemId, Viewer viewer)
        {
            try
            {
                IsLoading = true;
                LoadingProgress = 0;
                LoadingMessage = "Initialisation...";

                // STEP 1: Get JWT token and item details
                LoadingProgress = 5;
                LoadingMessage = "Authentification...";

                var jwtToken = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(jwtToken))
                {
                    LoadingMessage = "Erreur: Non authentifié";
                    IsLoading = false;
                    return Guid.Empty;
                }

                // STEP 2: Get inventory item details
                LoadingProgress = 10;
                LoadingMessage = "Récupération des détails de l'item...";

                ItemDetails = await _inventoryItemService.GetDetailsAsync(inventoryItemId, jwtToken);
                if (ItemDetails == null)
                {
                    LoadingMessage = "Erreur: Item introuvable";
                    IsLoading = false;
                    return Guid.Empty;
                }

                // STEP 3: Get 3D model data
                LoadingProgress = 15;
                LoadingMessage = "Récupération des données du modèle...";

                ThreeDModel weapon3DModelObject = await _threeDModelRepository.GetByIdAsync(ItemDetails.WearId);
                string weaponName = weapon3DModelObject.ItemModel;
                string pathModelOrLegacy = weapon3DModelObject.UvType == 1 ? "models/legacy/" : "models/model/";

                // STEP 4: Cache BIN file first and get its blob URL
                LoadingProgress = 25;
                LoadingMessage = "Mise en cache du fichier binaire...";

                await _cacheService.CacheLocalFile("/" + pathModelOrLegacy + weaponName + ".bin", weaponName + ".bin");
                var binBlobUrl = await _cacheService.GetCachedUrl(weaponName + ".bin");

                // STEP 5: Fetch and cache texture(s) from API
                LoadingProgress = 45;
                LoadingMessage = "Chargement des textures...";

                await _cacheService.FetchAndCacheImage(
#if DEBUG
                    "https://localhost:7009/api/wear/get3dmodel/" + ItemDetails.WearId,
#else
                    "https://apicsgoat-h7bhhpd4e7bnc9bh.eastus-01.azurewebsites.net/api/wear/get3dmodel/" + ItemDetails.WearId,
#endif
                    "applied_texture.png"
                );

                // STEP 6: Handle single or multiple textures
                LoadingProgress = 60;

                var textureUrls = new Dictionary<string, string>();

                if (weapon3DModelObject.Texture.Count == 1)
                {
                    var textureBlobUrl = await _cacheService.GetCachedUrl("applied_texture.png");
                    textureUrls["applied_texture.png"] = textureBlobUrl;
                }
                else if (weapon3DModelObject.Texture.Count == 2)
                {
                    await CacheTextureAsync(weapon3DModelObject.Texture[0], "applied_texture_l.png");
                    await CacheTextureAsync(weapon3DModelObject.Texture[1], "applied_texture_r.png");

                    var textureLBlobUrl = await _cacheService.GetCachedUrl("applied_texture_l.png");
                    var textureRBlobUrl = await _cacheService.GetCachedUrl("applied_texture_r.png");

                    textureUrls["applied_texture_l.png"] = textureLBlobUrl;
                    textureUrls["applied_texture_r.png"] = textureRBlobUrl;
                }

                // STEP 7: Handle SSG08 special case
                if (weaponName == "weapon_snip_ssg08" && weapon3DModelObject.UvType == 1)
                {
                    await _cacheService.CacheLocalFile("/models/ssg_lens.png", "ssg_lens.png");
                    var lensUrl = await _cacheService.GetCachedUrl("ssg_lens.png");
                    textureUrls["ssg_lens.png"] = lensUrl;

                    await _cacheService.CacheLocalFile("/models/ssg_scope.png", "ssg_scope.png");
                    var scopeUrl = await _cacheService.GetCachedUrl("ssg_scope.png");
                    textureUrls["ssg_scope.png"] = scopeUrl;
                }

                // STEP 8: Cache GLTF with modified URIs
                LoadingProgress = 75;
                LoadingMessage = "Préparation du modèle GLTF...";

                await _cacheService.CacheGltfWithBlobUrls(
                    "/" + pathModelOrLegacy + weaponName + ".gltf",
                    weaponName + ".gltf",
                    binBlobUrl,
                    textureUrls
                );

                // STEP 9: Get the modified GLTF blob URL
                LoadingProgress = 85;
                ModelUrl = await _cacheService.GetCachedUrl(weaponName + ".gltf");

                // STEP 10: Load the model
                LoadingProgress = 95;
                LoadingMessage = "Chargement du modèle 3D...";

                var settings = new ImportSettings
                {
                    Format = Import3DFormats.Gltf,
                    FileURL = ModelUrl,
                };

                LoadedObjectGuid = await viewer.Import3DModelAsync(settings);
                await viewer.SetCameraPositionAsync(new Vector3(45, 30, 0), new Vector3(0, 0, 0));

                return LoadedObjectGuid;
            }
            catch (Exception ex)
            {
                LoadingProgress = 100;
                LoadingMessage = $"Erreur: {ex.Message}";
                IsLoading = false;
                Console.WriteLine($"Error loading 3D model: {ex}");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Gère l'événement de chargement complet de l'objet 3D
        /// </summary>
        public async Task OnObjectLoaded(Object3DArgs e)
        {
            foreach (var item in Scene.Children)
            {
                if (item.Uuid == e.UUID)
                {
                    LoadingProgress = 100;
                    LoadingMessage = "Chargement terminé !";

                    await Task.Delay(500);
                    IsLoading = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Cache une texture en mémoire
        /// </summary>
        private async Task CacheTextureAsync(byte[] textureBytes, string cacheKey)
        {
            await _jsRuntime.InvokeAsync<bool>(
                "cacheHelper.putInCache",
                "model-cache",
                cacheKey,
                textureBytes
            );
        }

        /// <summary>
        /// Bascule le statut favori d'un item
        /// </summary>
        public async Task ToggleFavoriteAsync(int inventoryItemId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(token) && ItemDetails != null)
                {
                    ItemDetails.IsFavorite = !ItemDetails.IsFavorite;
                    OnPropertyChanged(nameof(ItemDetails));

                    await _inventoryItemService.ToggleFavoriteAsync(inventoryItemId, token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du toggle favori: {ex.Message}");
            }
        }
        public async Task DrawPriceHistoryGraph()
        {
            try
            {
                IsLoadingPriceHistory = true;
                ObservableCollection<PriceHistoryDTO>? allPriceHistory = await _priceHistoryService.GetByWear(ItemDetails.WearId);

                // Filtrer pour obtenir seulement les 30 derniers jours
                DateTime? thirtyDaysAgo = DateTime.Now.AddDays(-80);
                PriceHistory = new ObservableCollection<PriceHistoryDTO>(
                    allPriceHistory
                        .Where(p => p.PriceDate >= thirtyDaysAgo)
                        .OrderBy(p => p.PriceDate)
                        .ToList()
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'historique des prix: {ex.Message}");
                PriceHistory = new ObservableCollection<PriceHistoryDTO>();
            }
            finally
            {
                IsLoadingPriceHistory = false;
            }
        }
    }
    }
