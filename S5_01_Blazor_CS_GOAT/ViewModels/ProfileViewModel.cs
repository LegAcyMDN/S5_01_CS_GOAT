using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Profile - Gère l'état et la logique du profil utilisateur
    /// </summary>
    public class ProfileViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationManager _navigation;
        private readonly HttpClient _httpClient;

        private User? _currentUser;
        private UpdateUser _updateModel = new();
        private UpdateUser _passwordModel = new();
        private string _confirmPassword = string.Empty;
        private bool _isLoading = true;
        private bool _isSaving = false;
        private bool _isChangingPassword = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private string _passwordErrorMessage = string.Empty;
        private string _passwordSuccessMessage = string.Empty;

        public ProfileViewModel(AuthService authService, NavigationManager navigation, HttpClient httpClient)
        {
            _authService = authService;
            _navigation = navigation;
            _httpClient = httpClient;
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public UpdateUser UpdateModel
        {
            get => _updateModel;
            set => SetProperty(ref _updateModel, value);
        }

        public UpdateUser PasswordModel
        {
            get => _passwordModel;
            set => SetProperty(ref _passwordModel, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
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

        public bool IsChangingPassword
        {
            get => _isChangingPassword;
            set => SetProperty(ref _isChangingPassword, value);
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

        public string PasswordErrorMessage
        {
            get => _passwordErrorMessage;
            set => SetProperty(ref _passwordErrorMessage, value);
        }

        public string PasswordSuccessMessage
        {
            get => _passwordSuccessMessage;
            set => SetProperty(ref _passwordSuccessMessage, value);
        }

        public override async Task InitializeAsync()
        {
            await LoadUserProfileAsync();
        }

        public async Task LoadUserProfileAsync()
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
                    InitializeUpdateModel();
                    IsLoading = false;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = "Impossible de charger les informations du profil.";
                    IsLoading = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
                IsLoading = false;
            }
        }

        private void InitializeUpdateModel()
        {
            if (CurrentUser != null)
            {
                UpdateModel = new UpdateUser
                {
                    DisplayName = CurrentUser.DisplayName,
                    Email = CurrentUser.Email,
                    Phone = CurrentUser.Phone,
                    TwoFA = CurrentUser.TwoFA,
                    Seed = CurrentUser.Seed
                };
            }
        }

        public async Task SaveProfileAsync()
        {
            try
            {
                IsSaving = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var userId = await _authService.GetUserIdAsync();
                if (userId == null)
                {
                    ErrorMessage = "Session expirée. Veuillez vous reconnecter.";
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    ErrorMessage = "Session expirée. Veuillez vous reconnecter.";
                    return;
                }

                if (UpdateModel.TwoFA != CurrentUser?.TwoFA)
                {
                    if (string.IsNullOrWhiteSpace(UpdateModel.OldPassword))
                    {
                        ErrorMessage = "Veuillez entrer votre mot de passe actuel pour modifier le 2FA.";
                        return;
                    }

                    if (UpdateModel.TwoFA == 2 && (!CurrentUser.EmailIsVerified || string.IsNullOrEmpty(CurrentUser.Email)))
                    {
                        ErrorMessage = "Impossible d'activer le 2FA par e-mail : votre adresse e-mail n'est pas vérifiée.";
                        return;
                    }
                    else if (UpdateModel.TwoFA == 1 && (!CurrentUser.PhoneIsVerified || string.IsNullOrEmpty(CurrentUser.Phone)))
                    {
                        ErrorMessage = "Impossible d'activer le 2FA par téléphone : votre numéro de téléphone n'est pas vérifié.";
                        return;
                    }
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var profileUpdate = new UpdateUser
                {
                    DisplayName = UpdateModel.DisplayName,
                    Email = UpdateModel.Email,
                    Phone = UpdateModel.Phone,
                    TwoFA = UpdateModel.TwoFA,
                    Seed = UpdateModel.Seed,
                    OldPassword = UpdateModel.OldPassword,
                    NewPassword = null
                };

                var response = await _httpClient.PatchAsJsonAsync($"User/update", profileUpdate);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Profil mis à jour avec succès !";
                    if (UpdateModel.TwoFA != CurrentUser?.TwoFA)
                    {
                        SuccessMessage += " Le 2FA a été modifié.";
                    }
                    UpdateModel.OldPassword = null;
                    await LoadUserProfileAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Erreur lors de la mise à jour : {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }

        public async Task ChangePasswordAsync()
        {
            try
            {
                IsChangingPassword = true;
                PasswordErrorMessage = string.Empty;
                PasswordSuccessMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(PasswordModel.OldPassword))
                {
                    PasswordErrorMessage = "L'ancien mot de passe est requis.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(PasswordModel.NewPassword))
                {
                    PasswordErrorMessage = "Le nouveau mot de passe est requis.";
                    return;
                }

                if (PasswordModel.NewPassword.Length < 8)
                {
                    PasswordErrorMessage = "Le nouveau mot de passe doit contenir au moins 8 caractères.";
                    return;
                }

                if (PasswordModel.NewPassword != ConfirmPassword)
                {
                    PasswordErrorMessage = "Les mots de passe ne correspondent pas.";
                    return;
                }

                if (PasswordModel.OldPassword == PasswordModel.NewPassword)
                {
                    PasswordErrorMessage = "Le nouveau mot de passe doit être différent de l'ancien.";
                    return;
                }

                var userId = await _authService.GetUserIdAsync();
                if (userId == null)
                {
                    PasswordErrorMessage = "Session expirée. Veuillez vous reconnecter.";
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    PasswordErrorMessage = "Session expirée. Veuillez vous reconnecter.";
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PatchAsJsonAsync($"User/update", PasswordModel);

                if (response.IsSuccessStatusCode)
                {
                    PasswordSuccessMessage = "Mot de passe modifié avec succès !";
                    ResetPasswordForm();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    PasswordErrorMessage = $"Erreur lors du changement de mot de passe : {errorContent}";
                }
            }
            catch (Exception ex)
            {
                PasswordErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsChangingPassword = false;
            }
        }

        public void ResetForm()
        {
            InitializeUpdateModel();
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        public void ResetPasswordForm()
        {
            PasswordModel = new UpdateUser();
            ConfirmPassword = string.Empty;
            PasswordErrorMessage = string.Empty;
        }

        public async Task LogoutCurrentDeviceAsync()
        {
            await _authService.LogoutAsync();
            _navigation.NavigateTo("/login");
        }

        public async Task LogoutAllDevicesAsync()
        {
            await _authService.LogoutAsync();
            _navigation.NavigateTo("/login");
        }

        public void NavigateTo(string url)
        {
            _navigation.NavigateTo(url);
        }
    }
}
