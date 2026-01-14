using System.Text.Json.Serialization;

namespace S5_01_App_CS_GOAT.Services
{
    public class SteamUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        
        public SteamUserService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        
        public async Task<SteamUserData?> GetSteamUserDataAsync(string steamId)
        {
            var apiKey = _configuration["Steam:ApiKey"];
            var url = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={apiKey}&steamids={steamId}";
            
            try
            {
                var response = await _httpClient.GetFromJsonAsync<SteamApiResponse>(url);
                
                if (response?.Response?.Players?.Count > 0)
                {
                    var player = response.Response.Players[0];
                    return new SteamUserData
                    {
                        SteamId = player.SteamId,
                        Username = player.PersonaName,
                        AvatarUrl = player.AvatarFull,
                        ProfileUrl = player.ProfileUrl
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération des données Steam: {ex.Message}");
            }
            
            return null;
        }
    }

    // DTOs pour la réponse de l'API Steam
    public class SteamApiResponse
    {
        [JsonPropertyName("response")]
        public SteamResponse Response { get; set; } = null!;
    }

    public class SteamResponse
    {
        [JsonPropertyName("players")]
        public List<SteamPlayer> Players { get; set; } = new();
    }

    public class SteamPlayer
    {
        [JsonPropertyName("steamid")]
        public string SteamId { get; set; } = null!;
        
        [JsonPropertyName("personaname")]
        public string PersonaName { get; set; } = null!;
        
        [JsonPropertyName("profileurl")]
        public string ProfileUrl { get; set; } = null!;
        
        [JsonPropertyName("avatar")]
        public string Avatar { get; set; } = null!;
        
        [JsonPropertyName("avatarmedium")]
        public string AvatarMedium { get; set; } = null!;
        
        [JsonPropertyName("avatarfull")]
        public string AvatarFull { get; set; } = null!;
        
        [JsonPropertyName("personastate")]
        public int PersonaState { get; set; }
    }

    public class SteamUserData
    {
        public string SteamId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string AvatarUrl { get; set; } = null!;
        public string ProfileUrl { get; set; } = null!;
    }
}