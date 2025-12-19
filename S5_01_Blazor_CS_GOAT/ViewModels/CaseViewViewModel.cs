using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page CaseView - Gère l'ouverture des caisses
    /// </summary>
    public class CaseViewViewModel : ViewModelBase
    {
        private readonly IService<Skin> _skinRepository;
        private readonly IService<Case> _caseRepository;
        private static readonly Random _rng = new();

        private List<Skin> _skinsList = new();
        private Case? _activeCase;
        private List<Skin> _wonSkins = new();
        private bool _isEsthetic;
        private bool _showPopup;
        private int _selectedCount = 1;
        private bool _isLoading = true;

        public CaseViewViewModel(IService<Skin> skinRepository, IService<Case> caseRepository)
        {
            _skinRepository = skinRepository;
            _caseRepository = caseRepository;
        }

        public List<Skin> SkinsList
        {
            get => _skinsList;
            set => SetProperty(ref _skinsList, value);
        }

        public Case? ActiveCase
        {
            get => _activeCase;
            set => SetProperty(ref _activeCase, value);
        }

        public List<Skin> WonSkins
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
                // Animation Esthétique - à implémenter
            }

            Console.WriteLine($"Achat de {SelectedCount} cases");
            
            for (int i = 0; i < SelectedCount; i++)
            {
                int index = _rng.Next(SkinsList.Count);
                WonSkins.Add(SkinsList[index]);
            }

            ShowPopup = true;
            await Task.CompletedTask;
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
    }
}
