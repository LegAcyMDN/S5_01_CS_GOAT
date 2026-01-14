using S5_01_Blazor_CS_GOAT.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service générique pour gérer les appels avec GetOptions (pagination, recherche, filtres, tri)
    /// </summary>
    public class GetOptionsService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public GetOptionsService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        /// <summary>
        /// Effectue une requête GET avec les paramètres GetOptions
        /// </summary>
        public async Task<GetOptionsResponse<T>?> GetWithOptionsAsync<T>(
            string endpoint,
            string? searchTerm = null,
            string? sortKey = null,
            string? sortType = null,
            int? pageNumber = null,
            int? pageSize = null,
            Dictionary<string, List<string>>? filters = null) where T : class
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return null;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Construire l'URL avec les paramètres
                var queryParams = new List<string>();

                // Gestion de la recherche
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    queryParams.Add($"sorttype=search");
                    queryParams.Add($"sortkey={Uri.EscapeDataString(searchTerm)}");
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(sortKey))
                        queryParams.Add($"sortkey={Uri.EscapeDataString(sortKey)}");
                    
                    if (!string.IsNullOrWhiteSpace(sortType))
                        queryParams.Add($"sorttype={Uri.EscapeDataString(sortType)}");
                }

                // Pagination
                if (pageNumber.HasValue && pageNumber.Value > 0)
                    queryParams.Add($"page={pageNumber.Value}");

                if (pageSize.HasValue && pageSize.Value > 0)
                    queryParams.Add($"pagesize={pageSize.Value}");

                // Ajouter les filtres
                if (filters != null && filters.Any())
                {
                    foreach (var filter in filters)
                    {
                        if (filter.Value != null && filter.Value.Any())
                        {
                            var values = string.Join("+", filter.Value.Select(v => Uri.EscapeDataString(v)));
                            queryParams.Add($"{Uri.EscapeDataString(filter.Key.ToLower())}={values}");
                        }
                    }
                }

                var url = endpoint;
                if (queryParams.Any())
                    url += "?" + string.Join("&", queryParams);

                var response = await _httpClient.GetAsync(url);
                
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GetOptionsResponse<T>>();
                }

                Console.WriteLine($"Erreur HTTP {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur GetOptionsService: {ex.Message}");
                return null;
            }
        }
    }
}
