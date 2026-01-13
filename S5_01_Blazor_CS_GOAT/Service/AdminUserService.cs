using System.Net.Http.Headers;
using System.Net.Http.Json;
using Radzen;
using S5_01_Blazor_CS_GOAT.Models;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié à la gestion des utilisateurs pour l'administration
    /// Respecte le principe de responsabilité unique (SRP)
    /// </summary>
    public class AdminUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly GetOptionsService _getOptionsService;

        public AdminUserService(HttpClient httpClient, AuthService authService, GetOptionsService getOptionsService)
        {
            _httpClient = httpClient;
            _authService = authService;
            _getOptionsService = getOptionsService;
        }

        /// <summary>
        /// Récupère tous les utilisateurs
        /// </summary>
        public async Task<List<UserDTO>?> GetAllUsersAsync()
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return null;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.GetAsync("User/all");
            if (response.IsSuccessStatusCode)
            {
                var rt = await response.Content.ReadFromJsonAsync<GetOptionsResponse<UserDTO>>();
                return rt?.Result;
            }

            return null;
        }

        /// <summary>
        /// Récupère tous les utilisateurs avec pagination, tri, filtres et recherche
        /// </summary>
        public async Task<GetOptionsResponse<UserDTO>?> GetAllUsersWithOptionsAsync(
            string? searchTerm = null,
            string? sortKey = null,
            string? sortType = null,
            int? pageNumber = null,
            int? pageSize = null,
            Dictionary<string, List<string>>? filters = null)
        {
            return await _getOptionsService.GetWithOptionsAsync<UserDTO>(
                "User/all",
                searchTerm,
                sortKey,
                sortType,
                pageNumber,
                pageSize,
                filters
            );
        }

        /// <summary>
        /// Bascule le statut de bannissement d'un utilisateur
        /// </summary>
        public async Task<bool> ToggleBanAsync(int userId)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync($"Ban/toggle/{userId}", null);
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Supprime un utilisateur
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.DeleteAsync($"User/delete/{userId}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Bascule le rôle administrateur d'un utilisateur
        /// </summary>
        public async Task<bool> ToggleAdminRoleAsync(int userId)
        {
            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.PostAsync($"User/toggle-admin/{userId}", null);
            return response.IsSuccessStatusCode;
        }
    }
}
