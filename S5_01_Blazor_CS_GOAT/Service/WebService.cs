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
        }
        var response = await _httpClient.GetFromJsonAsync<GetOptionsResponse<TEntity>>($"{_endpoint}/all");
        return response.Result;
    }

    public async Task<GetOptionsResponse<TEntity>?> GetAllWithOptionsAsync(string? jwtToken, Dictionary<string, string>? queryParams = null)
    {
        if (!string.IsNullOrEmpty(jwtToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        string url = $"{_endpoint}/all";
        if (queryParams != null && queryParams.Any())
        {
            var queryString = string.Join("&", queryParams.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));
            url = $"{url}?{queryString}";
        }

        return await _httpClient.GetFromJsonAsync<GetOptionsResponse<TEntity>>(url);
    }

    /// <summary>
    /// Récupère toutes les entités avec filtrage des favoris géré intelligemment
    /// Cette méthode gère le cas spécial où le backend ne peut pas filtrer IsFavorite correctement
    /// </summary>
    public async Task<GetOptionsResponse<TEntity>?> GetAllWithFavoriteFilterAsync(string? jwtToken, Dictionary<string, string>? queryParams = null)
    {
        if (!string.IsNullOrEmpty(jwtToken))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);
        }

        // Vérifier si le filtre favoris est demandé
        bool filterByFavorites = queryParams?.ContainsKey("isfavorite") == true && 
                                 queryParams["isfavorite"].ToLower() == "true";

        if (!filterByFavorites)
        {
            // Pas de filtre favoris, utiliser la méthode normale
            return await GetAllWithOptionsAsync(jwtToken, queryParams);
        }

        // Extraire les paramètres de pagination
        int requestedPage = 1;
        int requestedPageSize = 25;
        
        if (queryParams?.ContainsKey("page") == true)
            int.TryParse(queryParams["page"], out requestedPage);
        
        if (queryParams?.ContainsKey("pagesize") == true)
            int.TryParse(queryParams["pagesize"], out requestedPageSize);

        // Créer une copie des paramètres sans le filtre isfavorite
        var backendParams = queryParams?.Where(kvp => kvp.Key.ToLower() != "isfavorite")
                                       .ToDictionary(kvp => kvp.Key, kvp => kvp.Value) 
                           ?? new Dictionary<string, string>();

        // Stratégie : Récupérer plusieurs pages jusqu'à avoir assez de favoris
        List<TEntity> allFavorites = new();
        int currentBackendPage = 1;
        int totalCount = 0;
        int maxPagesToFetch = 10; // Limite de sécurité pour éviter trop de requêtes
        
        while (allFavorites.Count < requestedPage * requestedPageSize && currentBackendPage <= maxPagesToFetch)
        {
            // Mettre à jour le numéro de page pour le backend
            backendParams["page"] = currentBackendPage.ToString();
            backendParams["pagesize"] = "100"; // Récupérer plus d'items par requête pour optimiser
            
            var response = await GetAllWithOptionsAsync(jwtToken, backendParams);
            
            if (response == null || response.Result == null || !response.Result.Any())
                break;

            // Garder le total count de la première requête
            if (currentBackendPage == 1)
                totalCount = response.TotalCount;

            // Filtrer les favoris en utilisant la réflexion pour accéder à IsFavorite
            var favorites = response.Result.Where(item =>
            {
                var isFavoriteProp = item?.GetType().GetProperty("IsFavorite");
                return isFavoriteProp != null && (bool)(isFavoriteProp.GetValue(item) ?? false);
            }).ToList();

            allFavorites.AddRange(favorites);
            
            // Si on a reçu moins d'items que demandé, on est à la dernière page
            if (response.Count < 100)
                break;
                
            currentBackendPage++;
        }

        // Calculer la pagination sur les favoris
        int totalFavorites = allFavorites.Count;
        int totalPages = (int)Math.Ceiling((double)totalFavorites / requestedPageSize);
        
        // Extraire la page demandée
        var pagedFavorites = allFavorites
            .Skip((requestedPage - 1) * requestedPageSize)
            .Take(requestedPageSize)
            .ToList();

        // Construire la réponse
        return new GetOptionsResponse<TEntity>
        {
            PropertyNames = new List<string>(),
            Result = pagedFavorites,
            Filters = queryParams?.Where(kvp => kvp.Key.ToLower() != "page" && 
                                               kvp.Key.ToLower() != "pagesize" &&
                                               kvp.Key.ToLower() != "pagenumber")
                                  .ToDictionary(kvp => kvp.Key, kvp => new List<string> { kvp.Value })
                     ?? new Dictionary<string, List<string>>(),
            SortKey = queryParams?.ContainsKey("sortkey") == true ? queryParams["sortkey"] : null,
            SortType = queryParams?.ContainsKey("sorttype") == true ? queryParams["sorttype"] : null,
            PageNumber = requestedPage,
            PageSize = requestedPageSize,
            Count = pagedFavorites.Count,
            PageCount = totalPages,
            FilteredCount = totalFavorites,
            TotalCount = totalCount,
            CanSearch = false,
            DtoTypeName = typeof(TEntity).Name
        };
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

    public async Task<List<TEntity>?> GetLiveFeedAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<GetOptionsResponse<TEntity>>($"{_endpoint}/livefeed");
        return response.Result;
    }
}