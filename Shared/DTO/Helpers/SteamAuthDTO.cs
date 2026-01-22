namespace Shared.DTO.Helpers;

/// <summary>
/// DTO for Steam authentication data
/// </summary>
public class SteamAuthDTO
{
    /// <summary>
    /// Steam ID (64-bit identifier)
    /// </summary>
    public string SteamId { get; set; } = null!;

    /// <summary>
    /// Steam username (persona name)
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// URL to user's Steam avatar image
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// URL to user's Steam profile
    /// </summary>
    public string? ProfileUrl { get; set; }
}