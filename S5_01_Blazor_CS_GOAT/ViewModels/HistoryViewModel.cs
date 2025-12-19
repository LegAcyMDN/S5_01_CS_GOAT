namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page History - Gère l'historique des transactions
    /// </summary>
    public class HistoryViewModel : ViewModelBase
    {
        private bool _isLoading = true;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public override async Task InitializeAsync()
        {
            // TODO: Charger l'historique des transactions
            IsLoading = false;
            await Task.CompletedTask;
        }
    }
}
