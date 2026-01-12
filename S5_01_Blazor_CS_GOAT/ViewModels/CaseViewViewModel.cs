using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using Shared.DTO.Helpers;
using Shared.Exceptions.CaseExceptions;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page CaseView - Refactorisé selon le principe SRP
    /// Responsabilité : Coordonner l'affichage et l'interaction avec les cases
    /// </summary>
    public class CaseViewViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly FavoriteService _favoriteService;
        private readonly IService<SkinDTO> _skinRepository;
        private readonly IService<CaseDTO> _caseRepository;
        private readonly IService<FairRandomDTO> _fairRandomService;
        private readonly CaseOpeningService _caseOpeningService;
        private readonly CaseResultMapperService _resultMapperService;

        private List<SkinDTO> _skinsList = new();
        private CaseDTO? _activeCase;
        private List<InventoryItemDetailDTO> _wonSkins = new();
        private bool _isEsthetic;
        private bool _showPopup;
        private int _selectedCount = 1;
        private string _promoCode = "";
        private bool _isLoading = true;
        private List<List<SkinDTO>> _boughtCasesListWithSkins = new();
        private List<InventoryItemDetailDTO> _listWonSkinItemDetail = new();
        private int _completedAnimations = 0;
        private int _totalAnimations = 0;
        private bool _isInvalidPromoCode = false;
        private bool _isAuthenticated = false;
        private string? _serverHash;
        private int? _userNonce;
        private MultipleCaseResultDTO _caseOpenResult;
        private bool _needAuth = false;

        public CaseViewViewModel(
            IService<SkinDTO> skinRepository,
            IService<CaseDTO> caseRepository,
            AuthService authService,
            FavoriteService favoriteService,
            IService<FairRandomDTO> fairRandomService,
            CaseOpeningService caseOpeningService,
            CaseResultMapperService resultMapperService)
        {
            _skinRepository = skinRepository;
            _caseRepository = caseRepository;
            _authService = authService;
            _favoriteService = favoriteService;
            _fairRandomService = fairRandomService;
            _caseOpeningService = caseOpeningService;
            _resultMapperService = resultMapperService;
        }

        #region Properties

        public string PromoCode
        {
            get => _promoCode;
            set => SetProperty(ref _promoCode, value);
        }

        public bool IsInvalidPromoCode
        {
            get => _isInvalidPromoCode;
            set => _isInvalidPromoCode = value;
        }

        public List<List<SkinDTO>> BoughtCasesListWithSkins
        {
            get => _boughtCasesListWithSkins;
            set => SetProperty(ref _boughtCasesListWithSkins, value);
        }

        public List<SkinDTO> SkinsList
        {
            get => _skinsList;
            set => SetProperty(ref _skinsList, value);
        }

        public CaseDTO? ActiveCase
        {
            get => _activeCase;
            set => SetProperty(ref _activeCase, value);
        }

        public List<InventoryItemDetailDTO> WonSkins
        {
            get => _wonSkins;
            set => SetProperty(ref _wonSkins, value);
        }

        public bool IsEsthetic
        {
            get => _isEsthetic;
            set => SetProperty(ref _isEsthetic, value);
        }

        public bool ShowPopup
        {
            get => _showPopup;
            set => SetProperty(ref _showPopup, value);
        }

        public int SelectedCount
        {
            get => _selectedCount;
            set => SetProperty(ref _selectedCount, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool JustBoughtCase
        {
            get => _completedAnimations < _totalAnimations && _totalAnimations > 0;
        }

        public List<InventoryItemDetailDTO> ListWonSkinItemDetail
        {
            get => _listWonSkinItemDetail;
            set => SetProperty(ref _listWonSkinItemDetail, value);
        }

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            set => SetProperty(ref _isAuthenticated, value);
        }

        public string? ServerHash
        {
            get => _serverHash;
            set => SetProperty(ref _serverHash, value);
        }

        public int? UserNonce
        {
            get => _userNonce;
            set => SetProperty(ref _userNonce, value);
        }

        public MultipleCaseResultDTO CaseOpenResult
        {
            get => _caseOpenResult;
            set => SetProperty(ref _caseOpenResult, value);
        }
        public bool NeedAuth
        {
            get => _needAuth;
            set => SetProperty(ref _needAuth, value);
        }

        #endregion

        /// <summary>
        /// Charge les données de la caisse et ses skins
        /// </summary>
        public async Task LoadCaseAsync(int caseId)
        {
            try
            {
                IsLoading = true;
                IsAuthenticated = await _authService.IsAuthenticatedAsync();

                var jwtToken = await _authService.GetTokenAsync();

                SkinsList = await _skinRepository.GetByCaseIdAsync(caseId);
                ActiveCase = await _caseRepository.GetByIdAsync(caseId, jwtToken);

                if (IsAuthenticated)
                {
                    await LoadFairRandomInfoAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de la caisse: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Charge les informations de provably fair pour l'utilisateur actuel
        /// </summary>
        private async Task LoadFairRandomInfoAsync()
        {
            try
            {
                var jwtToken = await _authService.GetTokenAsync();
                var fairRandomList = await _fairRandomService.GetByUserAsync(jwtToken);

                if (fairRandomList != null && fairRandomList.Count > 0)
                {
                    var latestFairRandom = fairRandomList.OrderByDescending(f => f.TransactionId).FirstOrDefault();

                    if (latestFairRandom != null)
                    {
                        ServerHash = latestFairRandom.ServerHash;
                        UserNonce = latestFairRandom.UserNonce;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des informations provably fair: {ex.Message}");
            }
        }

        public async Task TryBuyCaseAsync()
        {
            if (IsAuthenticated)
            {
                NeedAuth = false;
                await BuyCaseAsync();
            }
            else
            {
                NeedAuth = true;
            }
        }

        /// <summary>
        /// Achète une ou plusieurs caisses via le service dédié
        /// </summary>
        public async Task BuyCaseAsync()
        {

            if (SkinsList == null || SkinsList.Count == 0 || ActiveCase == null)
                return;

            WonSkins.Clear();
            ListWonSkinItemDetail.Clear();

            try
            {
                // Appel du service d'ouverture de cases
                CaseOpenResult = await _caseOpeningService.OpenCasesAsync(
                    ActiveCase.CaseId, 
                    SelectedCount, 
                    PromoCode);

                IsInvalidPromoCode = false;

                if (IsEsthetic)
                {
                    // Mode avec animation - préparer les données
                    BoughtCasesListWithSkins = _resultMapperService.ConvertMultipleCaseResultsToSkinLists(CaseOpenResult);
                    ListWonSkinItemDetail = _resultMapperService.ExtractWonSkins(CaseOpenResult);

                    _totalAnimations = BoughtCasesListWithSkins.Count;
                    _completedAnimations = 0;
                    OnPropertyChanged(nameof(JustBoughtCase));
                }
                else
                {
                    // Mode sans animation - afficher directement les résultats
                    WonSkins = _resultMapperService.ExtractWonSkins(CaseOpenResult);
                    ShowPopup = true;
                }
            }
            catch (InvalidPromoCodeException)
            {
                IsInvalidPromoCode = true;
            }
            catch (CaseOpeningException ex)
            {
                Console.WriteLine($"Erreur lors de l'ouverture de la caisse: {ex.Message}");
            }
        }

        /// <summary>
        /// Appelé quand une animation de case se termine
        /// </summary>
        public void OnCaseAnimationComplete()
        {
            _completedAnimations++;
            Console.WriteLine($"Completed: {_completedAnimations}/{_totalAnimations}");

            if (_completedAnimations >= _totalAnimations)
            {
                ShowResultsPopup();
            }
            
            OnPropertyChanged(nameof(JustBoughtCase));
        }

        /// <summary>
        /// Affiche la popup des résultats
        /// </summary>
        private void ShowResultsPopup()
        {
            Console.WriteLine("All animations complete! Showing results...");

            WonSkins = ListWonSkinItemDetail;
            ShowPopup = true;
        }

        /// <summary>
        /// Ferme la popup des skins gagnés
        /// </summary>
        public void ClosePopup()
        {
            ShowPopup = false;
        }

        /// <summary>
        /// Sélectionne le nombre de caisses à ouvrir
        /// </summary>
        public void SelectCount(int count)
        {
            SelectedCount = count;
        }

        /// <summary>
        /// Bascule le statut favori de la caisse active
        /// </summary>
        public async Task ToggleFavoriteAsync()
        {
            if (ActiveCase == null || !IsAuthenticated)
                return;

            bool success = await _favoriteService.ToggleFavoriteAsync(ActiveCase.CaseId, ActiveCase.IsFavorite);

            if (success)
            {
                ActiveCase.IsFavorite = !ActiveCase.IsFavorite;
                OnPropertyChanged(nameof(ActiveCase));
            }
        }
    }
}
