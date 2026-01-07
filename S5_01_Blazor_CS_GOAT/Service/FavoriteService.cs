using System.Net.Http.Headers;

namespace S5_01_Blazor_CS_GOAT.Service
{
    public class FavoriteService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public FavoriteService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task<bool> AddFavoriteAsync(int caseId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.PostAsync($"favorite/create/{caseId}", null);
                
                // Si le favori existe déjà (409), on considère que c'est un succès
                if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    Console.WriteLine($"Favorite already exists for case {caseId}");
                    return true;
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding favorite: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFavoriteAsync(int caseId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.DeleteAsync($"favorite/remove/{caseId}");
                
                // Si le favori n'existe pas (404), on considère que c'est un succès
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Favorite not found for case {caseId}");
                    return true;
                }
                
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing favorite: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ToggleFavoriteAsync(int caseId, bool currentState)
        {
            if (currentState)
            {
                return await RemoveFavoriteAsync(caseId);
            }
            else
            {
                return await AddFavoriteAsync(caseId);
            }
        }
    }
}
