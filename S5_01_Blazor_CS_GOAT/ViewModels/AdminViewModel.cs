using S5_01_Blazor_CS_GOAT.Service;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Admin - Refactorisé selon le principe SRP
    /// Responsabilité : Coordonner l'affichage de l'administration
    /// </summary>
    public class AdminViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly AdminUserService _adminUserService;
        private readonly TwoFactorAuthService _twoFactorAuthService;
        private readonly AdminStatisticsService _statisticsService;
        private readonly NavigationService _navigationService;

        private UserDTO? _currentUser;
        private List<UserDTO>? _allUsers;
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

        public AdminViewModel(
            AuthService authService,
            AdminUserService adminUserService,
            TwoFactorAuthService twoFactorAuthService,
            AdminStatisticsService statisticsService,
            NavigationService navigationService)
        {
            _authService = authService;
            _adminUserService = adminUserService;
            _twoFactorAuthService = twoFactorAuthService;
            _statisticsService = statisticsService;
            _navigationService = navigationService;
        }

        #region Properties

        public UserDTO? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public List<UserDTO>? AllUsers
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

        public List<UserDTO> FilteredUsers
        {
            get
            {
                if (AllUsers == null) return new List<UserDTO>();
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
                    _navigationService.NavigateToLogin();
                    return;
                }

                CurrentUser = _authService.CurrentUser;
                if (CurrentUser == null)
                {
                    await _authService.LoadCurrentUserAsync();
                    CurrentUser = _authService.CurrentUser;
                }
                
                if (CurrentUser == null || !CurrentUser.IsAdmin)
                {
                    ErrorMessage = "Accès refusé : vous n'êtes pas administrateur.";
                    _navigationService.NavigateToHome();
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
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
                IsLoading = false;
            }
        }

        /// <summary>
        /// Vérifie le code 2FA via le service dédié
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

                bool isValid = await _twoFactorAuthService.VerifyCodeAsync(TwoFACode);

                if (isValid)
                {
                    Is2FAVerified = true;
                    Requires2FA = false;
                    await LoadAdminDataAsync();
                }
                else
                {
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

        /// <summary>
        /// Charge la liste de tous les utilisateurs via le service
        /// </summary>
        public async Task LoadUsersAsync()
        {
            try
            {
                AllUsers = await _adminUserService.GetAllUsersAsync();
                TotalUsers = AllUsers?.Count ?? 0;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement des utilisateurs : {ex.Message}";
            }
        }

        /// <summary>
        /// Charge les statistiques via le service
        /// </summary>
        private async Task LoadStatisticsAsync()
        {
            try
            {
                double totalWalletAmount = AllUsers?.Sum(u => u.Wallet) ?? 0;
                var stats = await _statisticsService.GetStatisticsAsync(TotalUsers, totalWalletAmount);
                
                TotalTransactions = stats.TotalTransactions;
                TotalRevenue = stats.TotalRevenue;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des statistiques : {ex.Message}");
            }
        }

        /// <summary>
        /// Bannir/Débannir un utilisateur via le service
        /// </summary>
        public async Task ToggleBanUserAsync(int userId)
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                bool success = await _adminUserService.ToggleBanAsync(userId);
                
                if (success)
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
        /// Supprimer un utilisateur via le service
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

                bool success = await _adminUserService.DeleteUserAsync(userId);
                
                if (success)
                {
                    SuccessMessage = "Utilisateur supprimé avec succès.";
                    await LoadUsersAsync();
                }
                else
                {
                    ErrorMessage = "Erreur lors de la suppression.";
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
        /// Modifier le rôle admin d'un utilisateur via le service
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

                bool success = await _adminUserService.ToggleAdminRoleAsync(userId);
                
                if (success)
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
            _navigationService.NavigateTo(url);
        }
    }
}
