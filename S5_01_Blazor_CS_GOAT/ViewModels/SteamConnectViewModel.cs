using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;
using System.Net.Http.Headers;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    public class SteamConnectViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;
        private readonly HttpClient _httpClient;

        private UserDTO? _currentUser;
        private bool _isLoading = true;
        private bool _isSaving = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;

        public SteamConnectViewModel(
            AuthService authService,
            NavigationService navigationService,
            HttpClient httpClient)
        {
            _authService = authService;
            _navigationService = navigationService;
            _httpClient = httpClient;
        }

        #region Properties

        public UserDTO? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        #endregion

        #region Methods

        public async Task InitializeAsync()
        {
            await LoadUserDataAsync();
        }

        private async Task LoadUserDataAsync()
        {
            try
            {
                IsLoading = true;
                CurrentUser = _authService.CurrentUser;

                if (CurrentUser == null)
                {
                    await _authService.LoadCurrentUserAsync();
                    CurrentUser = _authService.CurrentUser;

                    if (CurrentUser == null)
                    {
                        _navigationService.NavigateToLogin();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des données : {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ConnectSteam()
        {
            string? apiBaseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');
            int userId = CurrentUser?.UserId ?? 0;
            string steamLoginUrl = $"{apiBaseUrl}/steam/login?linkUserId={userId}";
            _navigationService.NavigateTo(steamLoginUrl, forceLoad: true);
        }

        public async Task UnlinkSteamAsync()
        {
            try
            {
                IsSaving = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                string? token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "Session expirée. Veuillez vous reconnecter.";
                    return;
                }

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, "steam/unlink");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _navigationService.NavigateTo("/profile?success=steam_unlinked");
                }
                else
                {
                    ErrorMessage = $"Erreur lors du déliement du compte Steam. Code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du déliement du compte Steam : {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        public void NavigateTo(string url)
        {
            _navigationService.NavigateTo(url);
        }

        #endregion
    }
}
