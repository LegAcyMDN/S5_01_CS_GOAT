using System.Net.Http.Headers;
using System.Net.Http.Json;
using S5_01_Blazor_CS_GOAT.Models;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié à la gestion des codes promos pour l'administration
    /// </summary>
    public class AdminPromoCodeService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly GetOptionsService _getOptionsService;

        public AdminPromoCodeService(HttpClient httpClient, AuthService authService, GetOptionsService getOptionsService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _getOptionsService = getOptionsService;
        }

        /// <summary>
        /// Récupère tous les codes promos avec pagination, tri, filtres et recherche
        /// </summary>
        public async Task<GetOptionsResponse<PromoCodeDTO>?> GetAllPromoCodesWithOptionsAsync(
            string? searchTerm = null,
            string? sortKey = null,
            string? sortType = null,
            int? pageNumber = null,
            int? pageSize = null,
            Dictionary<string, List<string>>? filters = null)
        {
            return await _getOptionsService.GetWithOptionsAsync<PromoCodeDTO>(
                "PromoCode/all",
                searchTerm,
                sortKey,
                sortType,
                pageNumber,
                pageSize,
                filters
            );
        }

        /// <summary>
        /// Récupère tous les codes promos (sans pagination)
        /// </summary>
        public async Task<List<PromoCodeDTO>?> GetAllPromoCodesAsync()
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return null;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("PromoCode/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PromoCodeDTO>>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des codes promos : {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Crée un nouveau code promo
        /// </summary>
        public async Task<bool> CreatePromoCodeAsync(PromoCodeDTO promoCode)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("PromoCode/create", promoCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création du code promo : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Met à jour un code promo existant
        /// </summary>
        public async Task<bool> UpdatePromoCodeAsync(PromoCodeDTO promoCode)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"PromoCode/update/{promoCode.PromoCodeId}", promoCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la mise à jour du code promo : {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Supprime un code promo
        /// </summary>
        public async Task<bool> DeletePromoCodeAsync(int promoCodeId)
        {
            try
            {
                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return false;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"PromoCode/delete/{promoCodeId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression du code promo : {ex.Message}");
                return false;
            }
        }
    }
}
