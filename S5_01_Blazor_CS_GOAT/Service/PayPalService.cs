using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.Service
{
    public class PayPalService
    {
        private readonly HttpClient _httpClient;

        public PayPalService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CreateOrderResponse?> CreateOrderAsync(decimal amount, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.PostAsJsonAsync("paypal/create-order", new
            {
                amount = amount
            });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CreateOrderResponse>();
            }

            return null;
        }

        public async Task<CaptureOrderResponse?> CaptureOrderAsync(string orderId, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            var response = await _httpClient.PostAsync($"paypal/capture-order?orderId={orderId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<CaptureOrderResponse>();
            }

            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Capture failed: {error}");
            return null;
        }
    }

    public class CreateOrderResponse
    {
        public string OrderId { get; set; } = null!;
        public string? ApprovalUrl { get; set; }
        public string Status { get; set; } = null!;
    }

    public class CaptureOrderResponse
    {
        public bool Success { get; set; }
        public decimal Amount { get; set; }
        public double OldBalance { get; set; }
        public double NewBalance { get; set; }
        public string TransactionId { get; set; } = null!;
    }
}