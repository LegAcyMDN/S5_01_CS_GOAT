using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Admin - Gère l'administration du site
    /// </summary>
    public class AdminViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationManager _navigation;
        private readonly HttpClient _httpClient;

        private User? _currentUser;
        private List<User>? _allUsers;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private string _searchQuery = string.Empty;
        private bool _requires2FA = false;
        private bool _is2FAVerified = false;
        private string _twoFACode = string.Empty;
        private string _twoFAError = string.Empty;

        // Statistiques
        private int _totalUsers = 0;
        private int _totalTransactions = 0;
        private double _totalRevenue = 0;

        public AdminViewModel(AuthService authService, NavigationManager navigation, HttpClient httpClient)
        {
            _authService = authService;
            _navigation = navigation;
            _httpClient = httpClient;
        }

        #region Properties

        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public List<User>? AllUsers
        {
            get => _allUsers;
            set => SetProperty(ref _allUsers, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
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

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public bool Requires2FA
        {
            get => _requires2FA;
            set => SetProperty(ref _requires2FA, value);
        }

        public bool Is2FAVerified
        {
            get => _is2FAVerified;
            set => SetProperty(ref _is2FAVerified, value);
        }

        public string TwoFACode
        {
            get => _twoFACode;
            set => SetProperty(ref _twoFACode, value);
        }

        public string TwoFAError
        {
            get => _twoFAError;
            set => SetProperty(ref _twoFAError, value);
        }

        public int TotalUsers
        {
            get => _totalUsers;
            set => SetProperty(ref _totalUsers, value);
        }

        public int TotalTransactions
        {
            get => _totalTransactions;
            set => SetProperty(ref _totalTransactions, value);
        }

        public double TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public List<User> FilteredUsers
        {
            get
            {
                if (AllUsers == null) return new List<User>();
                if (string.IsNullOrWhiteSpace(SearchQuery)) return AllUsers;

                return AllUsers.Where(u =>
                    u.DisplayName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    u.Login.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (u.Email != null && u.Email.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
        }

        #endregion

        public override async Task InitializeAsync()
        {
            await CheckAdminAccessAsync();
        }

        /// <summary>
        /// Vérifie si l'utilisateur est admin et si 2FA est requis
        /// </summary>
        private async Task CheckAdminAccessAsync()
        {
            try
            {
                IsLoading = true;
                
                var userId = await _authService.GetUserIdAsync();
                if (userId == null)
                {
                    _navigation.NavigateTo("/login");
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigation.NavigateTo("/login");
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.GetAsync($"User/details/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    CurrentUser = await response.Content.ReadFromJsonAsync<User>();
                    
                    if (CurrentUser == null || !CurrentUser.IsAdmin)
                    {
                        ErrorMessage = "Accès refusé : vous n'êtes pas administrateur.";
                        _navigation.NavigateTo("/");
                        return;
                    }

                    // Vérifier si 2FA est activé
                    if (CurrentUser.TwoFA > 0)
                    {
                        Requires2FA = true;
                        IsLoading = false;
                    }
                    else
                    {
                        // Pas de 2FA, accès direct
                        Is2FAVerified = true;
                        await LoadAdminDataAsync();
                    }
                }
                else
                {
                    ErrorMessage = "Impossible de charger les informations utilisateur.";
                    _navigation.NavigateTo("/");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
                IsLoading = false;
            }
        }

        /// <summary>
        /// Vérifie le code 2FA
        /// </summary>
        public async Task Verify2FAAsync()
        {
            try
            {
                IsProcessing = true;
                TwoFAError = string.Empty;

                if (string.IsNullOrWhiteSpace(TwoFACode))
                {
                    TwoFAError = "Veuillez entrer le code 2FA.";
                    IsProcessing = false;
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigation.NavigateTo("/login");
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Appel API pour vérifier le code 2FA
                var verifyRequest = new { code = TwoFACode };
                var response = await _httpClient.PostAsJsonAsync("User/verify-2fa", verifyRequest);

                if (response.IsSuccessStatusCode)
                {
                    Is2FAVerified = true;
                    Requires2FA = false;
                    await LoadAdminDataAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TwoFAError = "Code 2FA incorrect. Veuillez réessayer.";
                }
            }
            catch (Exception ex)
            {
                TwoFAError = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Charge toutes les données d'administration
        /// </summary>
        private async Task LoadAdminDataAsync()
        {
            try
            {
                IsLoading = true;
                await LoadUsersAsync();
                await LoadStatisticsAsync();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des données : {ex.Message}";
                IsLoading = false;
            }
        }

        /// <summary>
        /// Charge la liste de tous les utilisateurs
        /// </summary>
        public async Task LoadUsersAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.GetAsync("User");
                if (response.IsSuccessStatusCode)
                {
                    AllUsers = await response.Content.ReadFromJsonAsync<List<User>>();
                    TotalUsers = AllUsers?.Count ?? 0;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des utilisateurs : {ex.Message}";
            }
        }

        /// <summary>
        /// Charge les statistiques générales
        /// </summary>
        private async Task LoadStatisticsAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                // Charger les transactions
                var transactionResponse = await _httpClient.GetAsync("Transaction");
                if (transactionResponse.IsSuccessStatusCode)
                {
                    var transactions = await transactionResponse.Content.ReadFromJsonAsync<List<dynamic>>();
                    TotalTransactions = transactions?.Count ?? 0;
                }

                // Calculer le revenu total à partir des wallets
                if (AllUsers != null)
                {
                    TotalRevenue = AllUsers.Sum(u => u.Wallet);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des statistiques : {ex.Message}");
            }
        }

        /// <summary>
        /// Bannir/Débannir un utilisateur
        /// </summary>
        public async Task ToggleBanUserAsync(int userId)
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.PostAsync($"Ban/toggle/{userId}", null);
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Statut de bannissement modifié avec succès.";
                    await LoadUsersAsync();
                }
                else
                {
                    ErrorMessage = "Erreur lors de la modification du statut.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Supprimer un utilisateur
        /// </summary>
        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (userId == CurrentUser?.UserId)
                {
                    ErrorMessage = "Vous ne pouvez pas supprimer votre propre compte.";
                    IsProcessing = false;
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.DeleteAsync($"User/delete/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Utilisateur supprimé avec succès.";
                    await LoadUsersAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Erreur lors de la suppression : {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Modifier le rôle admin d'un utilisateur
        /// </summary>
        public async Task ToggleAdminRoleAsync(int userId)
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (userId == CurrentUser?.UserId)
                {
                    ErrorMessage = "Vous ne pouvez pas modifier votre propre rôle admin.";
                    IsProcessing = false;
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                
                var response = await _httpClient.PostAsync($"User/toggle-admin/{userId}", null);
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Rôle administrateur modifié avec succès.";
                    await LoadUsersAsync();
                }
                else
                {
                    ErrorMessage = "Erreur lors de la modification du rôle.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        public void NavigateTo(string url)
        {
            _navigation.NavigateTo(url);
        }
    }
}
