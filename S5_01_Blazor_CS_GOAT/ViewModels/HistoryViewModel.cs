using Newtonsoft.Json.Linq;
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
        private UserDTO? _currentUser;
        private readonly IService<RandomTransactionDetailDTO> _randomTransactionRepository;
        private readonly IService<ItemTransactionDetailDTO> _itemTransactionRepository;
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;
        private List<RandomTransactionDetailDTO>? _randomsTransactionsDetail;
        private List<ItemTransactionDetailDTO>? _itemTransactionsDetail;
        private bool _isRandomTransactionsExpanded = true;
        private bool _isItemTransactionsExpanded = true;

        public HashSet<int> OpenTransactionIds { get; } = new();
        public HashSet<int> OpenItemTransactionIds { get; } = new();

        public HistoryViewModel(
            IService<RandomTransactionDetailDTO> randomTransactionRepository, 
            IService<ItemTransactionDetailDTO> itemTransactionRepository,
            AuthService authService, 
            HttpClient httpClient)
        {
            _randomTransactionRepository = randomTransactionRepository;
            _itemTransactionRepository = itemTransactionRepository;
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
        public List<ItemTransactionDetailDTO>? ItemTransactionsDetail
        {
            get => _itemTransactionsDetail;
            set => SetProperty(ref _itemTransactionsDetail, value);
        }
        public UserDTO? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool IsRandomTransactionsExpanded
        {
            get => _isRandomTransactionsExpanded;
            set => SetProperty(ref _isRandomTransactionsExpanded, value);
        }

        public bool IsItemTransactionsExpanded
        {
            get => _isItemTransactionsExpanded;
            set => SetProperty(ref _isItemTransactionsExpanded, value);
        }

        public override async Task InitializeAsync()
        {
            await LoadRandomsTransactionsDataAsync();
            await LoadItemTransactionsDataAsync();
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
                    RandomsTransactionsDetail.Sort((x, y) => y.TransactionDate.CompareTo(x.TransactionDate));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'historique des ouvertures: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadItemTransactionsDataAsync()
        {
            try
            {
                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    ItemTransactionsDetail = await _itemTransactionRepository.GetByUserAsync(jwtToken);
                    ItemTransactionsDetail?.Sort((x, y) => y.TransactionDate.CompareTo(x.TransactionDate));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement de l'historique des ventes: {ex.Message}");
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
                    {
                        OpenTransactionIds.Add(id);
                        Console.WriteLine($"Ajout {id} ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du detail: {ex.Message}");
            }
        }

        public async Task UnloadTransactionDetailsAsync(int id)
        {
            try
            {
                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    RandomTransactionDetailDTO detailsListe = await _randomTransactionRepository.GetDetailsAsync(id, jwtToken);

                    int index = RandomsTransactionsDetail.FindIndex(x => x.TransactionId == id);

                    if (detailsListe.TransactionId == id)
                    {
                        OpenTransactionIds.Remove(id);
                        Console.WriteLine($"Retrait {id} ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du detail: {ex.Message}");
            }
        }

        public void ToggleRandomTransactionsSection()
        {
            IsRandomTransactionsExpanded = !IsRandomTransactionsExpanded;
        }

        public void ToggleItemTransactionsSection()
        {
            IsItemTransactionsExpanded = !IsItemTransactionsExpanded;
        }

        public void ToggleItemTransactionDetails(int inventoryItemId)
        {
            if (OpenItemTransactionIds.Contains(inventoryItemId))
            {
                OpenItemTransactionIds.Remove(inventoryItemId);
            }
            else
            {
                OpenItemTransactionIds.Add(inventoryItemId);
            }
        }
    }
}
