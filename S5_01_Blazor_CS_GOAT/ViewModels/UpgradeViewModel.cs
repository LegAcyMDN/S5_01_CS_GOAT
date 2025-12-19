namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Upgrade - Gère l'amélioration des skins
    /// </summary>
    public class UpgradeViewModel : ViewModelBase
    {
        private bool _isLoading = true;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public override async Task InitializeAsync()
        {
            // TODO: Charger les skins disponibles pour l'upgrade
            IsLoading = false;
            await Task.CompletedTask;
        }
    }
}
