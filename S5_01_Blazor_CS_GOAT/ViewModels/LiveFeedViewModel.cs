using Shared.DTO;
using S5_01_Blazor_CS_GOAT.Service;
using System.Timers;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour LiveFeed - Gère le fil d'actualité des drops en direct
    /// </summary>
    public class LiveFeedViewModel : ViewModelBase, IDisposable
    {
        private readonly IService<RandomTransactionLiveFeedDTO> _randomTransactionService;
        private bool _showBestOnly = false;
        private List<LiveFeedItem> _feedItems = new();
        private System.Timers.Timer? _refreshTimer;
        private DateTime _lastUpdateTime = DateTime.UtcNow;
        private const int REFRESH_INTERVAL_MS = 5000; // Rafraîchir toutes les 5 secondes
        private const int MAX_ITEMS = 20; // Nombre max d'items à afficher

        public LiveFeedViewModel(IService<RandomTransactionLiveFeedDTO> randomTransactionService)
        {
            _randomTransactionService = randomTransactionService;
        }

        public bool ShowBestOnly
        {
            get => _showBestOnly;
            set
            {
                if (SetProperty(ref _showBestOnly, value))
                {
                    FilterFeedItems();
                }
            }
        }

        public List<LiveFeedItem> FeedItems
        {
            get => _feedItems;
            set => SetProperty(ref _feedItems, value);
        }

        public override async Task InitializeAsync()
        {
            await LoadFeedItemsAsync();
            StartAutoRefresh();
        }

        /// <summary>
        /// Démarre le rafraîchissement automatique
        /// </summary>
        private void StartAutoRefresh()
        {
            _refreshTimer = new System.Timers.Timer(REFRESH_INTERVAL_MS);
            _refreshTimer.Elapsed += async (sender, e) => await RefreshFeedAsync();
            _refreshTimer.AutoReset = true;
            _refreshTimer.Start();
        }

        /// <summary>
        /// Rafraîchit le feed avec les nouvelles transactions
        /// </summary>
        private async Task RefreshFeedAsync()
        {
            try
            {
                var transactions = await _randomTransactionService.GetLiveFeedAsync(50);
                
                if (transactions != null && transactions.Any())
                {
                    var newItems = transactions
                        .Where(t => t.TransactionDate > _lastUpdateTime)
                        .Select(t => new LiveFeedItem
                        {
                            ItemName = t.ItemName,
                            SkinName = t.SkinName,
                            RarityColor = t.RarityColor,
                            WearTypeAbbreviation = t.WearTypeAbbreviation,
                            Uuid = t.Uuid,
                            TransactionDate = t.TransactionDate,
                            IsNew = true
                        })
                        .OrderByDescending(item => item.TransactionDate)
                        .ToList();

                    if (newItems.Any())
                    {
                        // Ajouter les nouveaux items au début
                        var updatedList = newItems.Concat(_feedItems).ToList();
                        
                        // Limiter le nombre d'items
                        if (updatedList.Count > MAX_ITEMS)
                        {
                            updatedList = updatedList.Take(MAX_ITEMS).ToList();
                        }

                        // Marquer les anciens items comme non nouveaux
                        foreach (var item in updatedList.Skip(newItems.Count))
                        {
                            item.IsNew = false;
                        }

                        FeedItems = updatedList;
                        _lastUpdateTime = newItems.First().TransactionDate;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du rafraîchissement du live feed: {ex.Message}");
            }
        }

        /// <summary>
        /// Charge les items du fil d'actualité
        /// </summary>
        private async Task LoadFeedItemsAsync()
        {
            try
            {
                var transactions = await _randomTransactionService.GetLiveFeedAsync(MAX_ITEMS);
                
                if (transactions != null)
                {
                    FeedItems = transactions
                        .OrderByDescending(t => t.TransactionDate)
                        .Select(t => new LiveFeedItem
                        {
                            ItemName = t.ItemName,
                            SkinName = t.SkinName,
                            RarityColor = t.RarityColor,
                            WearTypeAbbreviation = t.WearTypeAbbreviation,
                            Uuid = t.Uuid,
                            TransactionDate = t.TransactionDate,
                            IsNew = false
                        }).ToList();

                    if (FeedItems.Any())
                    {
                        _lastUpdateTime = FeedItems.First().TransactionDate;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement du live feed: {ex.Message}");
            }
        }

        /// <summary>
        /// Filtre les items selon le mode d'affichage
        /// </summary>
        private void FilterFeedItems()
        {
            // TODO: Implémenter le filtrage des meilleurs drops
            // Pour l'instant, on recharge tout
            _ = LoadFeedItemsAsync();
        }

        public void Dispose()
        {
            _refreshTimer?.Stop();
            _refreshTimer?.Dispose();
        }

        /// <summary>
        /// Représente un item dans le fil d'actualité
        /// </summary>
        public class LiveFeedItem
        {
            public string ItemName { get; set; } = string.Empty;
            public string SkinName { get; set; } = string.Empty;
            public string RarityColor { get; set; } = string.Empty;
            public string WearTypeAbbreviation { get; set; } = string.Empty;
            public string Uuid { get; set; } = string.Empty;
            public DateTime TransactionDate { get; set; }
            public bool IsNew { get; set; } = false;

            public string DisplayName => $"{ItemName} | {SkinName}";
            
            public string ImageUrl => $"https://screenshots.cs.money/csmoney2/{Uuid}_icon.png";
        }
    }
}
