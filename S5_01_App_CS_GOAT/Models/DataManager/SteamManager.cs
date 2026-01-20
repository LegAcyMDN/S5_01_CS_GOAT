using System.Text.Json.Serialization;

using S5_01_App_CS_GOAT.Models.Repository;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.DataManager
{
    /// <summary>
    /// Provides integration with Steam API to retrieve user profile information
    /// </summary>
    public class SteamManager : ISteamRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        
        public SteamManager(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        
        public async Task<SteamUserData?> GetSteamUserDataAsync(string steamId)
        {
            string apiKey = _configuration["Steam:ApiKey"] ??
                throw new InvalidOperationException("Steam API key is not configured.");
            string url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}";
            
            SteamApiResponse? response = await _httpClient.GetFromJsonAsync<SteamApiResponse>(url);
                
            if (((response?.Response?.Players?.Count) ?? 0) > 0)
            {
                var player = response!.Response.Players[0];
                return new SteamUserData
                {
                    SteamId = player.SteamId,
                    Username = player.PersonaName,
                    AvatarUrl = player.AvatarFull,
                    ProfileUrl = player.ProfileUrl
                };
            }
            
            return null;
        }
    }
}