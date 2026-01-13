using S5_01_Blazor_CS_GOAT.Service;
using S5_01_Blazor_CS_GOAT.Models;
using Shared.DTO;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Profile - Conforme au principe SRP
    /// Responsabilité : Gérer l'affichage et l'édition du profil utilisateur
    /// </summary>
    public class ProfileViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;
        private readonly HttpClient _httpClient;

        private UserDTO? _currentUser;
        private bool _isLoading = true;
        private bool _isSaving = false;
        private bool _isChangingPassword = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private string _passwordErrorMessage = string.Empty;
        private string _passwordSuccessMessage = string.Empty;
        private string _confirmPassword = string.Empty;

        private UpdateUserModel _updateModel = new();
        private ChangePasswordModel _passwordModel = new();

        private string _verificationCode = string.Empty;
        private bool _isVerifying = false;
        private bool _showVerificationInput = false;
        private string _verificationType = string.Empty;

        public ProfileViewModel(
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public UpdateUserModel UpdateModel
        {
            get => _updateModel;
            set => SetProperty(ref _updateModel, value);
        }

        public ChangePasswordModel PasswordModel
        {
            get => _passwordModel;
            set => SetProperty(ref _passwordModel, value);
        }

        public string VerificationCode
        {
            get => _verificationCode;
            set => SetProperty(ref _verificationCode, value);
        }

        public bool IsVerifying
        {
            get => _isVerifying;
            set => SetProperty(ref _isVerifying, value);
        }

        public bool ShowVerificationInput
        {
            get => _showVerificationInput;
            set => SetProperty(ref _showVerificationInput, value);
        }

        public string VerificationType
        {
            get => _verificationType;
            set => SetProperty(ref _verificationType, value);
        }

        #endregion

        public override async Task InitializeAsync()
        {
            await LoadProfileAsync();
        }

        private async Task LoadProfileAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var userId = await _authService.GetUserIdAsync();
                if (userId == null)
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                await _authService.LoadCurrentUserAsync();
                CurrentUser = _authService.CurrentUser;

                if (CurrentUser != null)
                {
                    // Initialiser le modèle de mise à jour avec les valeurs actuelles
                    UpdateModel = new UpdateUserModel
                    {
                        DisplayName = CurrentUser.DisplayName,
                        Email = CurrentUser.Email,
                        Phone = CurrentUser.Phone,
                        TwoFA = CurrentUser.TwoFA,
                        Seed = CurrentUser.Seed ?? string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors du chargement du profil : {ex.Message}";
                Console.WriteLine($"Erreur LoadProfileAsync : {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void NavigateTo(string url)
        {
            _navigationService.NavigateTo(url);
        }

        /// <summary>
        /// Réinitialise le formulaire de profil
        /// </summary>
        public void ResetForm()
        {
            if (CurrentUser != null)
            {
                UpdateModel = new UpdateUserModel
                {
                    DisplayName = CurrentUser.DisplayName,
                    Email = CurrentUser.Email,
                    Phone = CurrentUser.Phone,
                    TwoFA = CurrentUser.TwoFA,
                    Seed = CurrentUser.Seed ?? string.Empty
                };
            }

            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        /// <summary>
        /// Réinitialise le formulaire de changement de mot de passe
        /// </summary>
        public void ResetPasswordForm()
        {
            PasswordModel = new ChangePasswordModel();
            ConfirmPassword = string.Empty;
            PasswordErrorMessage = string.Empty;
            PasswordSuccessMessage = string.Empty;
        }

        /// <summary>
        /// Sauvegarde les modifications du profil
        /// </summary>
        public async Task SaveProfileAsync()
        {
            try
            {
                IsSaving = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                // Validation
                if (string.IsNullOrWhiteSpace(UpdateModel.DisplayName))
                {
                    ErrorMessage = "Le nom d'affichage est requis.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(UpdateModel.Email) && string.IsNullOrWhiteSpace(UpdateModel.Phone))
                {
                    ErrorMessage = "Vous devez fournir au moins un email ou un numéro de téléphone.";
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PatchAsJsonAsync($"User/update", UpdateModel);
                
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Profil mis à jour avec succès !";
                    await _authService.LoadCurrentUserAsync();
                    CurrentUser = _authService.CurrentUser;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Erreur lors de la sauvegarde :  {errorContent}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la sauvegarde : {ex.Message}";
                Console.WriteLine($"Erreur SaveProfileAsync : {ex.Message}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        /// <summary>
        /// Change le mot de passe de l'utilisateur
        /// </summary>
        public async Task ChangePasswordAsync()
        {
            try
            {
                IsChangingPassword = true;
                PasswordErrorMessage = string.Empty;
                PasswordSuccessMessage = string.Empty;

                // Validation
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

                if (PasswordModel.NewPassword != ConfirmPassword)
                {
                    PasswordErrorMessage = "Les mots de passe ne correspondent pas.";
                    return;
                }

                if (PasswordModel.NewPassword.Length < 8)
                {
                    PasswordErrorMessage = "Le mot de passe doit contenir au moins 8 caractères.";
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PatchAsJsonAsync("User/update", PasswordModel);
                
                if (response.IsSuccessStatusCode)
                {
                    // Réinitialiser uniquement les champs du formulaire, pas les messages
                    PasswordModel = new ChangePasswordModel();
                    ConfirmPassword = string.Empty;
                    // Afficher le message de succès
                    PasswordSuccessMessage = "Mot de passe modifié avec succès !";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    PasswordErrorMessage = $"Erreur : {errorContent}";
                }
            }
            catch (Exception ex)
            {
                PasswordErrorMessage = $"Erreur : {ex.Message}";
                Console.WriteLine($"Erreur ChangePasswordAsync : {ex}");
            }
            finally
            {
                IsChangingPassword = false;
            }
        }

        /// <summary>
        /// Déconnecte l'utilisateur de l'appareil actuel
        /// </summary>
        public async Task LogoutCurrentDeviceAsync()
        {
            await _authService.LogoutAsync();
            _navigationService.NavigateToHome(forceLoad: true);
        }

        /// <summary>
        /// Déconnecte l'utilisateur de tous les appareils
        /// </summary>
        public async Task LogoutAllDevicesAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Appel API pour révoquer tous les tokens
                var response = await _httpClient.PostAsync("User/logout-all-devices", null);
                
                if (response.IsSuccessStatusCode)
                {
                    await _authService.LogoutAsync();
                    _navigationService.NavigateToHome(forceLoad: true);
                }
                else
                {
                    ErrorMessage = "Erreur lors de la déconnexion de tous les appareils.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
                Console.WriteLine($"Erreur LogoutAllDevicesAsync : {ex}");
            }
        }

        /// <summary>
        /// Demande un code de vérification pour l'email
        /// </summary>
        public async Task RequestEmailVerificationAsync()
        {
            await RequestVerificationCodeAsync("mail");
        }

        /// <summary>
        /// Demande un code de vérification pour le téléphone
        /// </summary>
        public async Task RequestPhoneVerificationAsync()
        {
            await RequestVerificationCodeAsync("sms");
        }

        /// <summary>
        /// Demande un code de vérification
        /// </summary>
        private async Task RequestVerificationCodeAsync(string contactType)
        {
            try
            {
                IsVerifying = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage(HttpMethod.Head, $"User/verify/{contactType}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    VerificationType = contactType;
                    ShowVerificationInput = true;
                    SuccessMessage = contactType == "mail" 
                        ? "Un code de vérification a été envoyé à votre adresse email." 
                        : "Un code de vérification a été envoyé par SMS.";
                }
                else
                {
                    ErrorMessage = $"Erreur lors de l'envoi du code de vérification. Code: {response.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de l'envoi du code : {ex.Message}";
                Console.WriteLine($"Erreur RequestVerificationCodeAsync : {ex}");
            }
            finally
            {
                IsVerifying = false;
            }
        }

        /// <summary>
        /// Vérifie le code de vérification
        /// </summary>
        public async Task VerifyCodeAsync()
        {
            try
            {
                IsVerifying = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(VerificationCode))
                {
                    ErrorMessage = "Veuillez entrer le code de vérification.";
                    return;
                }

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    _navigationService.NavigateToLogin();
                    return;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var request = new HttpRequestMessage(HttpMethod.Head, $"User/verify/{VerificationType}/{VerificationCode}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = VerificationType == "mail" 
                        ? "Email vérifié avec succès !" 
                        : "Téléphone vérifié avec succès !";
                    
                    ShowVerificationInput = false;
                    VerificationCode = string.Empty;
                    VerificationType = string.Empty;

                   
                    await LoadProfileAsync();
                }
                else
                {
                    ErrorMessage = response.StatusCode switch
                    {
                        System.Net.HttpStatusCode.Unauthorized => "Code de vérification invalide ou expiré.",
                        System.Net.HttpStatusCode.Gone => "Le code de vérification a expiré. Veuillez en demander un nouveau.",
                        _ => $"Erreur lors de la vérification. Code: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur lors de la vérification : {ex.Message}";
                Console.WriteLine($"Erreur VerifyCodeAsync : {ex}");
            }
            finally
            {
                IsVerifying = false;
            }
        }

        /// <summary>
        /// Annule la vérification en cours
        /// </summary>
        public void CancelVerification()
        {
            ShowVerificationInput = false;
            VerificationCode = string.Empty;
            VerificationType = string.Empty;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }
    }
}
