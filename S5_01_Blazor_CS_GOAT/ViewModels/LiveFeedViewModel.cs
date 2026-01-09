using Shared.DTO;
using S5_01_Blazor_CS_GOAT.Service;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour LiveFeed - Gère le fil d'actualité des drops en direct
    /// </summary>
    public class LiveFeedViewModel : ViewModelBase
    {
        private readonly IService<RandomTransactionLiveFeedDTO> _randomTransactionService;
        private bool _showBestOnly = false;
        private List<LiveFeedItem> _feedItems = new();

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
        }

        /// <summary>
        /// Charge les items du fil d'actualité
        /// </summary>
        private async Task LoadFeedItemsAsync()
        {
            try
            {
                var transactions = await _randomTransactionService.GetLiveFeedAsync(50);
                
                if (transactions != null)
                {
                    FeedItems = transactions.Select(t => new LiveFeedItem
                    {
                        ItemName = t.ItemName,
                        SkinName = t.SkinName,
                        RarityColor = t.RarityColor,
                        WearTypeAbbreviation = t.WearTypeAbbreviation,
                        Uuid = t.Uuid,
                        TransactionDate = t.TransactionDate
                    }).ToList();
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

            public string DisplayName => $"{ItemName} | {SkinName}";
            
            public string ImageUrl => $"https://screenshots.cs.money/csmoney2/{Uuid}_icon.png";
        }
    }
}
