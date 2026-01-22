using System.Net.Http.Json;
using System.Net.Http.Headers;
using S5_01_Blazor_CS_GOAT.Service;

public class StripeService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _appURL = "https://blazorcsgoat-hpbpdkhmadduekef.eastus-01.azurewebsites.net";

    public StripeService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<string?> CreateCheckoutSessionAsync(double amount)
    {
        var request = new
        {
            amount = amount,
            #if DEBUG
            successUrl = "https://localhost:7030/payment-success",
            cancelUrl =  "https://localhost:7030/wallet"
            #else 
            successUrl = _appURL + "/payment-success",
            cancelUrl = _appURL + "/wallet"
            #endif
        };

        var response = await _httpClient.PostAsJsonAsync("stripe/create-checkout-session", request);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
            return result?.Url;
        }

        return null;
    }
    
    public async Task<string?> CreateWithdrawalSessionAsync(double amount)
    {
        var token = await _authService.GetTokenAsync();
        if (string.IsNullOrEmpty(token)) return null;
        
        var request = new
        {
            amount = amount,
#if DEBUG
            successUrl = "https://localhost:7030/withdrawal-success",
            cancelUrl = "https://localhost:7030/wallet"
#else 
            successUrl = _appURL + "/withdrawal-success",
            cancelUrl = _appURL + "/wallet"
#endif
        };

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, "stripe/create-payout-session")
        {
            Content = JsonContent.Create(request),
            Headers =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", token)
            }
        };

        var response = await _httpClient.SendAsync(requestMessage);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
            return result?.Url;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
        }

        return null;
    }

    private class CheckoutResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}