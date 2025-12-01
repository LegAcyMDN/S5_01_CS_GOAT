using System.Net.Http.Json;

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
    
    public async Task<List<TEntity>?> GetByCaseIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<TEntity>?>($"{_endpoint}/bycase/{id}");
    }

    public async Task UpdateAsync(TEntity updatedEntity)
    {
        var idProp = typeof(TEntity).GetProperty("Id");
        if (idProp == null) throw new InvalidOperationException("Entity must have an Id property");

        var id = idProp.GetValue(updatedEntity);
        await _httpClient.PutAsJsonAsync($"{_endpoint}/update/{id}", updatedEntity);
    }
}