using System.Text.Json.Serialization;

namespace Shared.DTO.Helpers
{
    public class SteamApiResponse
    {
        [JsonPropertyName("response")]
        public SteamResponse Response { get; set; } = null!;
    }

    public class SteamResponse
    {
        [JsonPropertyName("players")]
        public List<SteamPlayer> Players { get; set; } = [];
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
