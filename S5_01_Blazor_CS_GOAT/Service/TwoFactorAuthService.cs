using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié à la vérification 2FA
    /// Respecte le principe de responsabilité unique (SRP)
    /// </summary>
    public class TwoFactorAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public TwoFactorAuthService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        /// <summary>
        /// Vérifie le code 2FA
        /// </summary>
        public async Task<bool> VerifyCodeAsync(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return false;

            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var verifyRequest = new { code };
            var response = await _httpClient.PostAsJsonAsync("User/verify-2fa", verifyRequest);

            return response.IsSuccessStatusCode;
        }
    }
}
