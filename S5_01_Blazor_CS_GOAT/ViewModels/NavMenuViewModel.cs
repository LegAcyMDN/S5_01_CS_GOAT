namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour NavMenu - Gère la navigation et les compteurs en ligne
    /// </summary>
    public class NavMenuViewModel : ViewModelBase
    {
        private int _onlineCount = 2048;

        public int OnlineCount
        {
            get => _onlineCount;
            set => SetProperty(ref _onlineCount, value);
        }

        /// <summary>
        /// Met à jour le nombre d'utilisateurs en ligne
        /// </summary>
        public async Task UpdateOnlineCountAsync()
        {
            // TODO: Implémenter la récupération du nombre d'utilisateurs en ligne depuis l'API
            await Task.CompletedTask;
        }
    }
}
