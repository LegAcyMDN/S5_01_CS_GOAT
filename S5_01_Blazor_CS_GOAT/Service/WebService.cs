using System.Net.Http.Json;
using System.Net.Http.Headers;
using Shared.DTO.Helpers;

namespace S5_01_Blazor_CS_GOAT.Service;

public class WebService<TEntity> : IService<TEntity> where TEntity : class
{
    private readonly HttpClient _httpClient;
    private string _endpoint;

    public WebService(string endpoint)
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

    public async Task AddAsync(TEntity entity)
    {
        await _httpClient.PostAsJsonAsync($"{_endpoint}/create", entity);
    }

    public async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"{_endpoint}/remove/{id}");
    }

    public async Task<List<TEntity>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<TEntity>?>($"{_endpoint}/all");
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TEntity?>($"{_endpoint}/details/{id}");
    }

    public async Task<TEntity?> GetByNameAsync(string name)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_endpoint}/search", name);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TEntity>();
    }
    
    //TODO remove that and make the system better
    public async Task<List<TEntity>?> GetByCaseIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<TEntity>?>($"{_endpoint}/bycase/{id}");
    }

    
    
    public async Task<List<MultipleCaseResultDTO>?> OpenCaseAsync(CaseOpenningDTO caseOpenInfo, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await _httpClient.PostAsJsonAsync($"{_endpoint}/open", caseOpenInfo);
    
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<List<MultipleCaseResultDTO>?>();
        }
    
        return null;
    }
    
    public async Task UpdateAsync(TEntity updatedEntity)
    {
        var idProp = typeof(TEntity).GetProperty("Id");
        if (idProp == null) throw new InvalidOperationException("Entity must have an Id property");

        var id = idProp.GetValue(updatedEntity);
        await _httpClient.PutAsJsonAsync($"{_endpoint}/update/{id}", updatedEntity);
    }

    public async Task<List<TEntity>?> GetByUserAsync(string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        return await _httpClient.GetFromJsonAsync<List<TEntity>?>($"{_endpoint}/byuser");
    }

    public async Task ToggleFavoriteAsync(int inventoryItemId, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        await _httpClient.PatchAsync($"{_endpoint}/togglefavorite/{inventoryItemId}", null);
    }

    public async Task<TEntity?> GetDetailsAsync(int inventoryItemId, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        return await _httpClient.GetFromJsonAsync<TEntity?>($"{_endpoint}/details/{inventoryItemId}");
    }
}