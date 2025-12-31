using System.Net.Http.Json;
using S5_01_Blazor_CS_GOAT.Service;

public class StripeService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;
    private readonly string _appURL = "https://apicsgoat-h7bhhpd4e7bnc9bh.eastus-01.azurewebsites.net";

    public StripeService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    public async Task<string?> CreateCheckoutSessionAsync(double amount)
    {
        int? userId = await _authService.GetUserIdAsync();
        
        if (userId == null)
        {
            Console.WriteLine("User not authenticated");
            return null;
        }
        
        var request = new
        {
            userId = userId.Value,
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
        int? userId = await _authService.GetUserIdAsync();
        
        if (userId == null)
        {
            Console.WriteLine("User not authenticated");
            return null;
        }
        
        var request = new
        {
            userId = userId.Value,
            amount = amount,
#if DEBUG
            successUrl = "https://localhost:7030/withdrawal-success",
            cancelUrl = "https://localhost:7030/wallet"
#else 
            successUrl = _appURL + "/withdrawal-success",
            cancelUrl = _appURL + "/wallet"
#endif
        };

        var response = await _httpClient.PostAsJsonAsync("stripe/create-payout-session", request);
        
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<CheckoutResponse>();
            return result?.Url;
        }

        return null;
    }

    private class CheckoutResponse
    {
        public string SessionId { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}