using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.Service;

public class ThreeDModelWebService<TEntity> : IThreeDModelService<TEntity> where TEntity : class
{
    private readonly HttpClient _httpClient;
    private string _endpoint;

    public ThreeDModelWebService(string endpoint)
    {
        _httpClient = new HttpClient
        {
#if DEBUG
            BaseAddress = new Uri("https://localhost:7009/api/")
#else
            BaseAddress = new Uri("https://apicsgoat-h7bhhpd4e7bnc9bh.eastus-01.azurewebsites.net/api/")
#endif
        };
        this._endpoint = endpoint;
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TEntity?>($"{_endpoint}/{id}");
    }

}