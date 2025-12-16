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
                // Store JWT token, userId and displayName
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", loginResponse.JwtToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", loginResponse.UserId.ToString());
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "displayName", loginResponse.DisplayName ?? "");
                
                // Store the remember token data (tokenId, tokenValue, expiry)
                if (loginResponse.RememberToken != null && !string.IsNullOrEmpty(loginResponse.RememberToken.TokenValue))
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberToken", loginResponse.RememberToken.TokenValue);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberTokenId", loginResponse.RememberToken.TokenId.ToString());
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberTokenExpiry", loginResponse.RememberToken.TokenExpiry.ToString("o"));
                }
                
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
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "rememberTokenId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "rememberTokenExpiry");
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
                            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberToken", authResponse.RememberToken.TokenValue);
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

        public async Task<int?> GetUserIdAsync()
        {
            var userIdString = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "userId");
            if (string.IsNullOrEmpty(userIdString))
            {
                return null;
            }
            
            if (int.TryParse(userIdString, out int userId))
            {
                return userId;
            }
            
            return null;
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
        
        public async Task InitializeAsync()
        {
            Console.WriteLine("=== Auth Initialize ===");
    
            // First, try to use JWT token if it exists
            var jwtToken = await GetTokenAsync();
            if (!string.IsNullOrEmpty(jwtToken))
            {
                Console.WriteLine("JWT token found, loading user...");
                await LoadCurrentUserAsync();
                return;
            }

            // If no JWT, try to use remember token
            Console.WriteLine("No JWT, checking remember token...");
            await TryLoginWithRememberTokenAsync();
        }

private async Task<bool> TryLoginWithRememberTokenAsync()
{
    try
    {
        // Get stored remember token data
        var rememberToken = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "rememberToken");
        var userIdString = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "userId");
        var tokenIdString = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "rememberTokenId");
        
        if (string.IsNullOrEmpty(rememberToken) || string.IsNullOrEmpty(userIdString))
        {
            Console.WriteLine("No remember token or userId found");
            return false;
        }

        if (!int.TryParse(userIdString, out int userId))
        {
            Console.WriteLine("Invalid userId format");
            return false;
        }

        // TokenId might not be stored yet, default to 0 if not found
        int tokenId = 0;
        if (!string.IsNullOrEmpty(tokenIdString))
        {
            int.TryParse(tokenIdString, out tokenId);
        }

        Console.WriteLine($"Remember token found for userId: {userId}, attempting recall...");
        
        // Call API recall endpoint
        var request = new
        {
            userId = userId,
            tokenId = tokenId,
            token = rememberToken
        };

        var response = await _httpClient.PostAsJsonAsync("user/recall", request);

        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            
            if (loginResponse != null)
            {
                Console.WriteLine("Token recall successful!");
                
                // Store new JWT token and user info
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", loginResponse.JwtToken);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", loginResponse.UserId.ToString());
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "displayName", loginResponse.DisplayName ?? "");
                
                // Store new remember token if provided
                if (loginResponse.RememberToken != null && !string.IsNullOrEmpty(loginResponse.RememberToken.TokenValue))
                {
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberToken", loginResponse.RememberToken.TokenValue);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberTokenId", loginResponse.RememberToken.TokenId.ToString());
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "rememberTokenExpiry", loginResponse.RememberToken.TokenExpiry.ToString("o"));
                }
                
                await LoadCurrentUserAsync();
                return true;
            }
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Remember token recall failed: {errorContent}");
            
            // Remember token is invalid or expired, clean up
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "rememberToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "rememberTokenId");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "rememberTokenExpiry");
            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error recalling token: {ex.Message}");
        return false;
    }
    
    return false;
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
        public RememberTokenResponse? RememberToken { get; set; }
    }
    
    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
    }
    
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string? DisplayName { get; set; }
        public string JwtToken { get; set; } = string.Empty;
        public RememberTokenResponse? RememberToken { get; set; }
    }

    public class RememberTokenResponse
    {
        public int TokenId { get; set; }
        public string TokenValue { get; set; } = string.Empty;
        public DateTime TokenCreationDate { get; set; }
        public DateTime TokenExpiry { get; set; }
        public int UserId { get; set; }
    }
}