using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié à la navigation dans l'application
    /// Respecte le principe de responsabilité unique (SRP)
    /// </summary>
    public class NavigationService
    {
        private readonly NavigationManager _navigationManager;

        public NavigationService(NavigationManager navigationManager)
        {
            _navigationManager = navigationManager;
        }

        public void NavigateTo(string url, bool forceLoad = false)
        {
            _navigationManager.NavigateTo(url, forceLoad);
        }

        public void NavigateToLogin() => NavigateTo("/login");
        
        public void NavigateToRegister() => NavigateTo("/register");
        
        public void NavigateToHome(bool forceLoad = false) => NavigateTo("/", forceLoad);
        
        public void NavigateToWallet() => NavigateTo("/wallet");
        
        public void NavigateToProfile() => NavigateTo("/profile");
        
        public void NavigateToAdmin() => NavigateTo("/admin");
        
        public void NavigateToCase(int caseId) => NavigateTo($"/caseview/{caseId}");
        
        public void NavigateToItemDetail(int inventoryItemId) => NavigateTo($"/viewmodel/{inventoryItemId}");
    }
}
