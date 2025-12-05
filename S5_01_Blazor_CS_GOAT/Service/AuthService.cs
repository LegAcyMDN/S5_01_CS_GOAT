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
                        // Store token
                        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "jwtToken", loginResponse.Token);
                        
                        return new LoginResult { Success = true, Token = loginResponse.Token };
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
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userName");
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