using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Register - Gère l'inscription utilisateur
    /// </summary>
    public class RegisterViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationManager _navigation;
        private readonly HttpClient _httpClient;

        private string _login = string.Empty;
        private string _displayName = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string? _email;
        private string? _phone;
        private bool _remember = false;
        private string _errorMessage = string.Empty;
        private bool _isLoading = false;
        private Dictionary<string, string> _validationErrors = new();

        public RegisterViewModel(AuthService authService, NavigationManager navigation, HttpClient httpClient)
        {
            _authService = authService;
            _navigation = navigation;
            _httpClient = httpClient;
        }

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string? Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        public bool Remember
        {
            get => _remember;
            set => SetProperty(ref _remember, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public Dictionary<string, string> ValidationErrors
        {
            get => _validationErrors;
            set => SetProperty(ref _validationErrors, value);
        }

        /// <summary>
        /// Valide tous les champs du formulaire
        /// </summary>
        private void ValidateForm()
        {
            ValidationErrors.Clear();

            // Validation du login
            if (string.IsNullOrWhiteSpace(Login))
                ValidationErrors["Login"] = "Le login est requis";
            else if (Login.Length < 3)
                ValidationErrors["Login"] = "Le login doit contenir au moins 3 caractères";
            else if (!Regex.IsMatch(Login, @"^[a-zA-Z0-9_-]+$"))
                ValidationErrors["Login"] = "Le login ne peut contenir que des lettres, chiffres, tirets et underscores";

            // Validation du nom d'affichage
            if (string.IsNullOrWhiteSpace(DisplayName))
                ValidationErrors["DisplayName"] = "Le nom d'affichage est requis";
            else if (DisplayName.Length < 2)
                ValidationErrors["DisplayName"] = "Le nom d'affichage doit contenir au moins 2 caractères";

            // Validation du mot de passe
            if (string.IsNullOrWhiteSpace(Password))
                ValidationErrors["Password"] = "Le mot de passe est requis";
            else if (Password.Length < 8)
                ValidationErrors["Password"] = "Le mot de passe doit contenir au moins 8 caractères";
            else
            {
                var passwordErrors = new List<string>();
                if (!Regex.IsMatch(Password, @"[A-Z]")) passwordErrors.Add("une majuscule");
                if (!Regex.IsMatch(Password, @"[a-z]")) passwordErrors.Add("une minuscule");
                if (!Regex.IsMatch(Password, @"[0-9]")) passwordErrors.Add("un chiffre");
                if (!Regex.IsMatch(Password, @"[!@#$%^&*(),.?""':{}|<>]")) passwordErrors.Add("un caractère spécial");

                if (passwordErrors.Count > 0)
                    ValidationErrors["Password"] = $"Le mot de passe doit contenir au moins {string.Join(", ", passwordErrors)}";
            }

            // Validation de la confirmation du mot de passe
            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                ValidationErrors["ConfirmPassword"] = "Veuillez confirmer le mot de passe";
            else if (Password != ConfirmPassword)
                ValidationErrors["ConfirmPassword"] = "Les mots de passe ne correspondent pas";

            // Validation email ou téléphone requis
            if (string.IsNullOrWhiteSpace(Email) && string.IsNullOrWhiteSpace(Phone))
            {
                ValidationErrors["Email"] = "Veuillez fournir un email ou un numéro de téléphone";
                ValidationErrors["Phone"] = "Veuillez fournir un email ou un numéro de téléphone";
            }
        }

        /// <summary>
        /// Gère l'inscription de l'utilisateur
        /// </summary>
        public async Task HandleRegisterAsync()
        {
            try
            {
                ValidateForm();

                if (ValidationErrors.Count > 0)
                    return;

                IsLoading = true;
                ErrorMessage = string.Empty;

                var rememberDays = Remember ? 30 : 0;
                var result = await _authService.RegisterAsync(
                    Login,
                    DisplayName,
                    Password,
                    Email,
                    Phone,
                    rememberDays
                );

                if (result.Success)
                {
                    await _authService.LoadCurrentUserAsync();
                    _navigation.NavigateTo("/", forceLoad: true);
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Inscription échouée";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Une erreur est survenue: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
