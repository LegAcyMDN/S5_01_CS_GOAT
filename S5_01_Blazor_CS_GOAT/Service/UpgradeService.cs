using System.Net.Http.Json;
using System.Net.Http.Headers;
using Shared.DTO.Helpers;

namespace S5_01_Blazor_CS_GOAT.Service
{
    public class UpgradeService
    {
        private readonly HttpClient _httpClient;

        public UpgradeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UpgradeOutputDTO?> ExecuteUpgradeAsync(UpgradeInputDTO input, string jwtToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            
            var response = await _httpClient.PostAsJsonAsync("inventoryitem/upgrade", input);
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UpgradeOutputDTO>();
            }
            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erreur lors de l'amelioration: {errorContent}");
        }
    }
}
