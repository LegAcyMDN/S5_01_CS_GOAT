using System.Net.Http.Json;
using Microsoft.JSInterop;
using S5_01_Blazor_CS_GOAT.Models;

namespace S5_01_Blazor_CS_GOAT.Service
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly string _apiUrl;

        private User? _currentUser;
        public User? CurrentUser => _currentUser;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            
#if DEBUG
            _apiUrl = "https://localhost:7009/api/";
#else
            _apiUrl = "https://apicsgoat-h7bhhpd4e7bnc9bh.eastus-01.azurewebsites.net/api/";
#endif
            _httpClient.BaseAddress = new Uri(_apiUrl);
        }

        public async Task<LoginResult> LoginAsync(string identifier, string password, int rememberDays)
        {
            try
            {
                var request = new
                {
                    identifier,
                    password,
                    remember = rememberDays
                };

                var response = await _httpClient.PostAsJsonAsync("user/login", request);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    
                    if (loginResponse != null)
                    {
                        // Store token, userId and displayName
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", loginResponse.JwtToken);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", loginResponse.UserId.ToString());
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "displayName", loginResponse.DisplayName ?? "");
                        
                        if (loginResponse.RememberToken != null)
                        {
                            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberToken", loginResponse.RememberToken.Value);
                        }
                        
                        // Charger les informations complètes de l'utilisateur
                        await LoadCurrentUserAsync();
                        
                        return new LoginResult { Success = true, Token = loginResponse.JwtToken };
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new LoginResult { Success = false, ErrorMessage = $"Login failed: {errorContent}" };
            }
            catch (Exception ex)
            {
                return new LoginResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "jwtToken");
        }

        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwtToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "displayName");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "rememberToken");
            _currentUser = null;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
        
        public async Task<RegisterResult> RegisterAsync(string login, string displayName, string password, string? email, string? phone, int rememberDays)
        {
            try
            {
                var request = new
                {
                    login,
                    displayName,
                    password,
                    email,
                    phone,
                    remember = rememberDays
                };

                var response = await _httpClient.PostAsJsonAsync("user/create", request);

                if (response.IsSuccessStatusCode)
                {
                    var authResponse = await response.Content.ReadFromJsonAsync<AuthDTO>();
                    
                    if (authResponse != null)
                    {
                        // Store tokens and user info
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", authResponse.JwtToken);
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", authResponse.UserId.ToString());
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "displayName", authResponse.DisplayName ?? "");
                        
                        if (authResponse.RememberToken != null)
                        {
                            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberToken", authResponse.RememberToken.Value);
                        }
                        
                        return new RegisterResult { Success = true, AuthData = authResponse };
                    }
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                return new RegisterResult { Success = false, ErrorMessage = errorContent };
            }
            catch (Exception ex)
            {
                return new RegisterResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        public async Task<bool> IsLoggedInAsync()
        {
            var token = await GetTokenAsync();
            return !string.IsNullOrEmpty(token);
        }

        public async Task<string?> GetUserPseudoAsync()
        {
            if (_currentUser != null)
            {
                return _currentUser.DisplayName;
            }
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "displayName");
        }

        public async Task LoadCurrentUserAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                var userIdString = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "userId");
                
                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userIdString))
                {
                    _currentUser = null;
                    return;
                }

                if (!int.TryParse(userIdString, out int userId))
                {
                    _currentUser = null;
                    return;
                }

                // Ajouter le token à l'en-tête
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync($"user/details/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    _currentUser = await response.Content.ReadFromJsonAsync<User>();
                }
                else
                {
                    _currentUser = null;
                }
            }
            catch
            {
                _currentUser = null;
            }
        }

        public class RegisterResult
        {
            public bool Success { get; set; }
            public AuthDTO? AuthData { get; set; }
            public string? ErrorMessage { get; set; }
        }

        public class AuthDTO
        {
            public int UserId { get; set; }
            public string? DisplayName { get; set; }
            public string JwtToken { get; set; } = string.Empty;
            public Token? RememberToken { get; set; }
        }

        public class Token
        {
            public string Value { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
}