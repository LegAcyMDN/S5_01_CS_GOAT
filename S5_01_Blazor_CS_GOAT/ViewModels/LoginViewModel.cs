using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Login - Refactorisé selon le principe SRP
    /// Responsabilité : Gérer l'interface de connexion utilisateur
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly NavigationService _navigationService;

        private string _identifier = string.Empty;
        private string _password = string.Empty;
        private bool _remember = false;
        private string _errorMessage = string.Empty;
        private string _identifierError = string.Empty;
        private string _passwordError = string.Empty;
        private bool _isLoading = false;

        public LoginViewModel(AuthService authService, NavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        public string Identifier
        {
            get => _identifier;
            set
            {
                if (SetProperty(ref _identifier, value))
                {
                    ValidateIdentifier();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    ValidatePassword();
                }
            }
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

        public string IdentifierError
        {
            get => _identifierError;
            set => SetProperty(ref _identifierError, value);
        }

        public string PasswordError
        {
            get => _passwordError;
            set => SetProperty(ref _passwordError, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        /// <summary>
        /// Valide le champ identifiant
        /// </summary>
        private void ValidateIdentifier()
        {
            if (string.IsNullOrWhiteSpace(Identifier))
            {
                IdentifierError = "L'identifiant est requis";
            }
            else
            {
                IdentifierError = string.Empty;
            }
        }

        /// <summary>
        /// Valide le champ mot de passe
        /// </summary>
        private void ValidatePassword()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Le mot de passe est requis";
            }
            else
            {
                PasswordError = string.Empty;
            }
        }

        /// <summary>
        /// Gère la connexion de l'utilisateur
        /// </summary>
        public async Task HandleLoginAsync()
        {
            try
            {
                // Validation avant soumission
                ValidateIdentifier();
                ValidatePassword();

                if (!string.IsNullOrEmpty(IdentifierError) || !string.IsNullOrEmpty(PasswordError))
                {
                    return;
                }

                IsLoading = true;
                ErrorMessage = string.Empty;

                var rememberDays = Remember ? 30 : 0;
                var result = await _authService.LoginAsync(Identifier, Password, rememberDays);

                if (result.Success)
                {
                    _navigationService.NavigateToHome();
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "Login failed";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Navigue vers la page d'inscription
        /// </summary>
        public void NavigateToRegister() => _navigationService.NavigateToRegister();
    }
}
