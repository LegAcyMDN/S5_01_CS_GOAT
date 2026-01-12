using S5_01_Blazor_CS_GOAT.Models;
using Shared.DTO.Helpers;
using Shared.Enum;
using Shared.Exceptions.CaseExceptions;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.Service;

public class WebService<TEntity> : IService<TEntity> where TEntity : class
{
    private readonly HttpClient _httpClient;
    private string _endpoint;

    public WebService(IConfiguration configuration, string endpoint)
    {
        var apiBaseUrl = configuration["ApiBaseUrl"]
                         ?? throw new InvalidOperationException("API Base URL not configured in appsettings");

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(apiBaseUrl)
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

    public async Task<List<TEntity>?> GetAllAsync(string? jwtToken)
    {
        if (!string.IsNullOrEmpty(jwtToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
            var response = await _httpClient.GetFromJsonAsync<GetOptionsResponse<TEntity>>($"{_endpoint}/all");
            return response.Result;
        }
        return await _httpClient.GetFromJsonAsync<List<TEntity>?>($"{_endpoint}/all");
    }

    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<TEntity?>($"{_endpoint}/details/{id}");
    }

    public async Task<TEntity?> GetByIdAsync(int id, string? jwtToken)
    {
        if (!string.IsNullOrEmpty(jwtToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }
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
         var response = await _httpClient.GetFromJsonAsync<GetOptionsResponse<TEntity>>($"{_endpoint}/bycase/{id}");
        return response.Result;
    }

    public async Task<MultipleCaseResultDTO?> OpenCaseAsync(CaseOpenningDTO caseOpenInfo, string jwtToken)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        var response = await _httpClient.PostAsJsonAsync($"{_endpoint}/open", caseOpenInfo);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<MultipleCaseResultDTO?>();
        }

        var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        string errorMessage = errorResponse?["message"];

        switch (errorMessage)
        {
            case "User not found":
                throw new UserNotFoundException();
            case "Quantity must be greater than zero.":
                throw new InvalidQuantityException();
            case "Case not found.":
                throw new CaseNotFoundException();
            case "Promo code is invalid.":
                throw new InvalidPromoCodeException();
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
        var response = await _httpClient.GetFromJsonAsync<GetOptionsResponse<TEntity>?>($"{_endpoint}/byuser");
        return response.Result;
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
    public async Task<ObservableCollection<TEntity>?> GetByWear(int wearId)
    {
        return await _httpClient.GetFromJsonAsync<ObservableCollection<TEntity>?>($"{_endpoint}/bywear/{wearId}");
    }

    public async Task<List<TEntity>?> GetLiveFeedAsync(int count = 20)
    {
        return await _httpClient.GetFromJsonAsync<List<TEntity>?>($"{_endpoint}/livefeed?count={count}");
    }
}