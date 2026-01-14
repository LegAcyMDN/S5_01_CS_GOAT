using System.Net.Http.Json;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.Service;

public class SteamAuthService
{
    private readonly HttpClient _httpClient;
    
    public SteamAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/auth/user");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
    
    public async Task<UserDTO> GetUserInfoAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<UserDTO>("/api/auth/user");
        return response;
    }
}