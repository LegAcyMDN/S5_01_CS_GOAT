using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié au calcul des statistiques administratives
    /// Respecte le principe de responsabilité unique (SRP)
    /// </summary>
    public class AdminStatisticsService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;

        public AdminStatisticsService(HttpClient httpClient, AuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        /// <summary>
        /// Charge les statistiques générales
        /// </summary>
        public async Task<AdminStats> GetStatisticsAsync(int totalUsers, double totalWalletAmount)
        {
            var stats = new AdminStats
            {
                TotalUsers = totalUsers,
                TotalRevenue = totalWalletAmount
            };

            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
                return stats;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var transactionResponse = await _httpClient.GetAsync("Transaction");
                if (transactionResponse.IsSuccessStatusCode)
                {
                    var transactions = await transactionResponse.Content.ReadFromJsonAsync<List<dynamic>>();
                    stats.TotalTransactions = transactions?.Count ?? 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des statistiques : {ex.Message}");
            }

            return stats;
        }
    }

    /// <summary>
    /// Classe contenant les statistiques administratives
    /// </summary>
    public class AdminStats
    {
        public int TotalUsers { get; set; }
        public int TotalTransactions { get; set; }
        public double TotalRevenue { get; set; }
    }
}
