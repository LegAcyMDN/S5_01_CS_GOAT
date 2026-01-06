using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using Shared.DTO.Helpers;
using Shared.Exceptions.CaseExceptions;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page CaseView - Gère l'ouverture des caisses
    /// </summary>
    public class CaseViewViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly IService<SkinDTO> _skinRepository;
        private readonly IService<Case> _caseRepository;
        private static readonly Random _rng = new();

        private List<SkinDTO> _skinsList = new();
        private Case? _activeCase;
        private List<InventoryItemDetailDTO> _wonSkins = new();
        private bool _isEsthetic;
        private bool _showPopup;
        private int _selectedCount = 1;
        private string _promoCode = "";
        private bool _isLoading = true;
        private List<Skin> _caseOpenList = new();
        private bool _justBoughtCase  = false;
        private List<List<SkinDTO>> _boughtCasesListWithSkins = new();
        private List<InventoryItemDetailDTO> _listWonSkinItemDetail = new();
        
        private int _completedAnimations = 0;
        private int _totalAnimations = 0;
        
        private bool _isInvalidPromoCode = false;



        public CaseViewViewModel(IService<SkinDTO> skinRepository, IService<Case> caseRepository, AuthService authService)
        {
            _skinRepository = skinRepository;
            _caseRepository = caseRepository;
            _authService = authService;
        }


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

        public Case? ActiveCase
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
            get => _justBoughtCase;
            set => SetProperty(ref _justBoughtCase, value);
        }

        public List<InventoryItemDetailDTO> ListWonSkinItemDetail
        {
            get => _listWonSkinItemDetail;
            set => SetProperty(ref _listWonSkinItemDetail, value);
        }

        /// <summary>
        /// Charge les données de la caisse et ses skins
        /// </summary>
        public async Task LoadCaseAsync(int caseId)
        {
            try
            {
                IsLoading = true;
                SkinsList = await _skinRepository.GetByCaseIdAsync(caseId);
                ActiveCase = await _caseRepository.GetByIdAsync(caseId);
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
        /// Achète une ou plusieurs caisses et génère les skins gagnés
        /// </summary>
        public async Task BuyCaseAsync()
        {
            if (SkinsList == null || SkinsList.Count == 0)
                return;

            WonSkins.Clear();

            if (IsEsthetic)
            {
                Console.WriteLine(SelectedCount);
                try
                {
                    MultipleCaseResultDTO casesObj = await CallCaseOpen(SelectedCount);
                    BoughtCasesListWithSkins = convertMultipleCaseResultsToSkinList(casesObj);

                    foreach (var c in casesObj.Results)
                    {
                        ListWonSkinItemDetail.Add(c.Reward);
                    }

                    _totalAnimations = BoughtCasesListWithSkins.Count;
                    _completedAnimations = 0;

                    IsInvalidPromoCode = false;
                    JustBoughtCase = true;
                }
                catch (CaseOpeningException e)
                {
                    switch (e)
                    {
                        case InvalidPromoCodeException:
                            IsInvalidPromoCode = true;
                            break;
                        // Add the other exceptions if needed
                    }
                }
            }
            else
            {
                // Non-esthetic mode - skip animations, show results immediately
                Console.WriteLine($"Opening {SelectedCount} cases without animation");
                try
                {
                var results = await CallCaseOpen(SelectedCount);
                IsInvalidPromoCode = false;
                // Extract won skins directly from results
                foreach (var oneCase in results.Results)
                {
                    var wonSkin = oneCase.Reward;
                    WonSkins.Add(new InventoryItemDetailDTO
                    {
                        Uuid = wonSkin.Uuid,
                        ItemName = wonSkin.ItemName,
                        RarityColor = wonSkin.RarityColor,
                        RarityName = wonSkin.RarityName,
                        SkinName = wonSkin.SkinName,
                        LastPrice = 1 //TODO make it good
                    });
                }
        
                // Show popup immediately
                ShowPopup = true;

                }
                catch (CaseOpeningException e)
                {
                    switch (e)
                    {
                        case InvalidPromoCodeException:
                            IsInvalidPromoCode = true;
                            break;
                        // Add the other exceptions if needed
                    }
                }
        

            }
        }
        
        public void OnCaseAnimationComplete()
        {
            _completedAnimations++;
            Console.WriteLine($"Completed: {_completedAnimations}/{_totalAnimations}");
    
            if (_completedAnimations >= _totalAnimations)
            {
                // All animations done - show popup!
                ShowResultsPopup();
            }
        }

        private void ShowResultsPopup()
        {
            Console.WriteLine("All animations complete! Showing results...");

            // Extract won skins from the roller results
            //foreach (var caseList in BoughtCasesListWithSkins)
            //{
            //    // The won skin is at position 72 (middle of the 82 items)
            //    if (caseList.Count > 72)
            //    {
            //        WonSkins.Add(caseList);
            //    }
            //}

            foreach (var reward in ListWonSkinItemDetail)
            {
                WonSkins.Add(reward);
            }
            
            ShowPopup = true;
            JustBoughtCase = false; // Hide the rollers
            
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
        
        
        
        private async Task<MultipleCaseResultDTO?> CallCaseOpen(int numberOfCases)
        {
            CaseOpenningDTO caseOpenningInfo = new CaseOpenningDTO
            {
                CaseId = _activeCase.CaseId,
                Quantity = numberOfCases,
                RaffleRollerLength = 82,
                PromoCode = PromoCode == "" ? null : PromoCode
            };
            
            string jwtToken = await _authService.GetTokenAsync();
            MultipleCaseResultDTO casesReturn = await _caseRepository.OpenCaseAsync(caseOpenningInfo, jwtToken);
    
            // Refresh wallet
            await _authService.LoadCurrentUserAsync();
    
            // This will trigger the event and update the menu!
            _authService.NotifyUserDataChanged();

            return casesReturn;

        }

        private List<List<SkinDTO>> convertMultipleCaseResultsToSkinList(MultipleCaseResultDTO multipleCaseResults)
        {
            List<List<SkinDTO>> casesWithSkins = new();
            int caseNumber = 0;
            foreach (var oneCase in multipleCaseResults.Results)
            {
                casesWithSkins.Add(new List<SkinDTO>());
                for (int i = 0; i <= 71; i++)
                {
                    SkinDTO skinOfThisIteration = multipleCaseResults.Skins[
                        oneCase.Roller[i]
                    ];
                    
                    casesWithSkins[caseNumber].Add(skinOfThisIteration);
                }

                // Add the skin won from the API
                InventoryItemDetailDTO wonSkinDetail = oneCase.Reward;
                
                casesWithSkins[caseNumber].Add(new SkinDTO
                {
                    AnyUuid = wonSkinDetail.Uuid,
                    ItemName = wonSkinDetail.ItemName,
                    RarityColor = wonSkinDetail.RarityColor,
                    RarityName = wonSkinDetail.RarityName,
                    SkinName =  wonSkinDetail.SkinName,
                    BestPrice = 1, // dummy numbers because we dont use them here
                    WorstPrice = 1 // dummy numbers because we dont use them here
                } );
                Console.WriteLine(oneCase.Roller.Length);
                Console.WriteLine("item found UUID : "  + wonSkinDetail.Uuid);
                Console.WriteLine("item : "   + wonSkinDetail.ItemName);
                
                
                for (int i = 72; i < 82; i++)
                {
                    SkinDTO skinOfThisIteration = multipleCaseResults.Skins[
                        oneCase.Roller[i]
                    ];
                    
                    casesWithSkins[caseNumber].Add(skinOfThisIteration);
                }
                
                
                caseNumber++;
            }

            Console.WriteLine("cases with skins : " + casesWithSkins[0].Count);
            return casesWithSkins;
        }



        
    }
}
