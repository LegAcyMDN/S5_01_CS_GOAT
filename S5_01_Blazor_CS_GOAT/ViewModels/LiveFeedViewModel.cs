namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour LiveFeed - Gère le fil d'actualité des drops en direct
    /// </summary>
    public class LiveFeedViewModel : ViewModelBase
    {
        private bool _showBestOnly = false;
        private List<LiveFeedItem> _feedItems = new();

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
            // TODO: Implémenter le chargement depuis l'API
            // Pour l'instant, données de démonstration
            FeedItems = Enumerable.Range(0, 23)
                .Select(i => new LiveFeedItem
                {
                    ItemName = "AK47 | Asimov",
                    Price = 256.64m
                })
                .ToList();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Filtre les items selon le mode d'affichage
        /// </summary>
        private void FilterFeedItems()
        {
            // TODO: Implémenter le filtrage des meilleurs drops
        }

        /// <summary>
        /// Représente un item dans le fil d'actualité
        /// </summary>
        public class LiveFeedItem
        {
            public string ItemName { get; set; } = string.Empty;
            public decimal Price { get; set; }
        }
    }
}
