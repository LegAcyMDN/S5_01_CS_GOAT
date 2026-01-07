using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page History - Gère l'historique des transactions
    /// </summary>
    public class HistoryViewModel : ViewModelBase
    {
        private bool _isLoading = true;
        private User? _currentUser;
        private readonly IService<RandomTransactionDetailDTO> _randomTransactionRepository;
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;
        private List<RandomTransactionDetailDTO>? _randomsTransactionsDetail;

        public HashSet<int> OpenTransactionIds { get; } = new();

        public HistoryViewModel(IService<RandomTransactionDetailDTO> randomTransactionRepository, AuthService authService, HttpClient httpClient)
        {
            _randomTransactionRepository = randomTransactionRepository;
            _authService = authService;
            _httpClient = httpClient;
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        public List<RandomTransactionDetailDTO>? RandomsTransactionsDetail 
        { 
            get => _randomsTransactionsDetail;
            set => SetProperty(ref _randomsTransactionsDetail, value);
        }
        public User? CurrentUser 
        { 
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public override async Task InitializeAsync()
        {
            await LoadRandomsTransactionsDataAsync();
            IsLoading = false;
            await Task.CompletedTask;
        }

        private async Task LoadRandomsTransactionsDataAsync()
        {
            try
            {
                IsLoading = true;

                await UpdateUserStateAsync();

                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    RandomsTransactionsDetail = await _randomTransactionRepository.GetByUserAsync(jwtToken);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'historique: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdateUserStateAsync()
        {
            if (_authService.CurrentUser == null && await _authService.IsLoggedInAsync())
            {
                await _authService.LoadCurrentUserAsync();
            }

            CurrentUser = _authService.CurrentUser;
        }

        public async Task LoadTransactionDetailsAsync(int id)
        {
            try
            {
                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    RandomTransactionDetailDTO detailsListe = await _randomTransactionRepository.GetDetailsAsync(id, jwtToken);

                    int index = RandomsTransactionsDetail.FindIndex(x => x.TransactionId == id);

                    if (index != -1)
                    {
                        RandomsTransactionsDetail[index] = detailsListe;
                    }

                    if (!OpenTransactionIds.Contains(id))
                        OpenTransactionIds.Add(id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du detail: {ex.Message}");
            }
        }
    }
}
