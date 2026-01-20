using S5_01_App_CS_GOAT.Models.DataManager;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Models.Repository
{
    /// <summary>
    /// Integrates with the Steam Web API to retrieve player profile details
    /// </summary>
    public interface ISteamRepository
    {
        /// <summary>
        /// Fetches Steam user profile data using the player's Steam ID
        /// </summary>
        /// <param name="steamId">The 64-bit Steam ID for the target player</param>
        /// <returns>Profile data including persona name, avatar, and profile URL; null if not found</returns>
        Task<SteamUserData?> GetSteamUserDataAsync(string steamId);
    }
}
