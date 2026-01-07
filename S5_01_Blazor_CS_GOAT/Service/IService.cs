using Shared.DTO.Helpers;
using System.Collections.ObjectModel;

namespace S5_01_Blazor_CS_GOAT.Service;

public interface IService<TEntity>
{
    Task<List<TEntity>?> GetAllAsync();
    Task<List<TEntity>?> GetAllAsync(string? jwtToken);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> GetByIdAsync(int id, string? jwtToken);
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity updatedEntity);
    Task DeleteAsync(int id);
    Task<TEntity?> GetByNameAsync(string name);
    
    Task<List<TEntity>?> GetByCaseIdAsync(int id);
    Task<MultipleCaseResultDTO?> OpenCaseAsync(CaseOpenningDTO caseOpenInfo, string jwtToken);
    Task<List<TEntity>?> GetByUserAsync(string jwtToken);
    Task ToggleFavoriteAsync(int inventoryItemId, string jwtToken);
    Task<TEntity?> GetDetailsAsync(int inventoryItemId, string jwtToken);
    Task<ObservableCollection<TEntity>?> GetByWear(int wearId);
}