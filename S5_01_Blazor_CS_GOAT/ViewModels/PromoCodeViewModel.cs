using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shared.DTO;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la gestion des codes promos dans l'admin
    /// </summary>
    public class PromoCodeViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly AdminPromoCodeService _promoCodeService;

        private List<PromoCodeDTO>? _allPromoCodes;
        private PromoCodeDTO? _selectedPromoCode;
        private PromoCodeDTO _editingPromoCode = new PromoCodeDTO();
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private string _searchQuery = string.Empty;
        private bool _showCreateModal = false;
        private bool _showEditModal = false;
        
        // Pagination et filtres
        private int _currentPage = 1;
        private int _pageSize = 25;
        private int _totalPages = 1;
        private int _totalCount = 0;
        private int _filteredCount = 0;
        private string? _sortKey = null;
        private string _sortType = "asc";
        private Dictionary<string, List<string>> _filters = new();

        public PromoCodeViewModel(AuthService authService, AdminPromoCodeService promoCodeService)
        {
            _authService = authService;
            _promoCodeService = promoCodeService;
        }

        #region Properties

        public List<PromoCodeDTO>? AllPromoCodes
        {
            get => _allPromoCodes;
            set => SetProperty(ref _allPromoCodes, value);
        }

        public PromoCodeDTO? SelectedPromoCode
        {
            get => _selectedPromoCode;
            set => SetProperty(ref _selectedPromoCode, value);
        }

        public PromoCodeDTO EditingPromoCode
        {
            get => _editingPromoCode;
            set => SetProperty(ref _editingPromoCode, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set => SetProperty(ref _searchQuery, value);
        }

        public bool ShowCreateModal
        {
            get => _showCreateModal;
            set => SetProperty(ref _showCreateModal, value);
        }

        public bool ShowEditModal
        {
            get => _showEditModal;
            set => SetProperty(ref _showEditModal, value);
        }

        // Propriétés de pagination
        public int CurrentPage
        {
            get => _currentPage;
            set => SetProperty(ref _currentPage, value);
        }

        public int PageSize
        {
            get => _pageSize;
            set => SetProperty(ref _pageSize, value);
        }

        public int TotalPages
        {
            get => _totalPages;
            set => SetProperty(ref _totalPages, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public int FilteredCount
        {
            get => _filteredCount;
            set => SetProperty(ref _filteredCount, value);
        }

        public string? SortKey
        {
            get => _sortKey;
            set => SetProperty(ref _sortKey, value);
        }

        public string SortType
        {
            get => _sortType;
            set => SetProperty(ref _sortType, value);
        }

        public Dictionary<string, List<string>> Filters
        {
            get => _filters;
            set => SetProperty(ref _filters, value);
        }

        /// <summary>
        /// Calcule la prochaine date de rafraîchissement pour un code promo
        /// </summary>
        public DateTime? GetNextRefresh(PromoCodeDTO promo)
        {
            if (promo.RefreshDelay == null || promo.ExpiryDate == null) return null;
            return promo.ExpiryDate.Value.Add(promo.RefreshDelay.Value);
        }

        /// <summary>
        /// Vérifie si un code promo est expiré
        /// </summary>
        public bool IsExpired(PromoCodeDTO promo)
        {
            return promo.ExpiryDate.HasValue && DateTime.Now > promo.ExpiryDate.Value;
        }

        /// <summary>
        /// Vérifie si un code promo est prêt à être rafraîchi
        /// </summary>
        public bool IsDueForRefresh(PromoCodeDTO promo)
        {
            if (!IsExpired(promo)) return false;
            if (promo.RefreshDelay == null) return false;
            var nextRefresh = GetNextRefresh(promo);
            return nextRefresh.HasValue && DateTime.Now >= nextRefresh.Value;
        }

        /// <summary>
        /// Vérifie si un code promo sera automatiquement supprimé
        /// </summary>
        public bool WillBeDeleted(PromoCodeDTO promo)
        {
            if (promo.RefreshDelay != null) return false;
            return IsExpired(promo) || promo.RemainingUses == 0;
        }

        /// <summary>
        /// Obtient le statut d'un code promo avec sa description
        /// </summary>
        public (string Status, string Description, string CssClass) GetPromoStatus(PromoCodeDTO promo)
        {
            var isNotStarted = promo.ValidityStart > DateTime.Now;
            var isExpired = IsExpired(promo);
            var willBeDeleted = WillBeDeleted(promo);
            var dueForRefresh = IsDueForRefresh(promo);

            if (willBeDeleted)
            {
                return ("🗑️ À supprimer", "Ce code sera automatiquement supprimé", "status-to-delete");
            }
            if (dueForRefresh)
            {
                var nextRefresh = GetNextRefresh(promo);
                return ("🔄 À rafraîchir", $"Sera rafraîchi le {nextRefresh:dd/MM/yyyy HH:mm}", "status-refresh");
            }
            if (isNotStarted)
            {
                return ("⏳ En attente", $"Début le {promo.ValidityStart:dd/MM/yyyy HH:mm}", "status-pending");
            }
            if (isExpired)
            {
                if (promo.RefreshDelay != null)
                {
                    var nextRefresh = GetNextRefresh(promo);
                    return ("⏰ Expiré", $"Sera rafraîchi le {nextRefresh:dd/MM/yyyy HH:mm}", "status-expired-refresh");
                }
                return ("❌ Expiré", "Ce code a expiré", "status-expired");
            }
            if (promo.RemainingUses == 0)
            {
                if (promo.RefreshDelay != null)
                {
                    return ("🔄 Épuisé", "Sera réinitialisé lors du rafraîchissement", "status-depleted-refresh");
                }
                return ("🚫 Épuisé", "Aucune utilisation restante", "status-depleted");
            }
            return ("✅ Actif", "Code promo actif et utilisable", "status-active");
        }

        #endregion

        public override async Task InitializeAsync()
        {
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Charge tous les codes promos avec pagination
        /// </summary>
        public async Task LoadPromoCodesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var response = await _promoCodeService.GetAllPromoCodesWithOptionsAsync(
                    searchTerm: !string.IsNullOrWhiteSpace(SearchQuery) ? SearchQuery : null,
                    sortKey: SortKey,
                    sortType: SortType,
                    pageNumber: CurrentPage,
                    pageSize: PageSize,
                    filters: Filters.Any() ? Filters : null
                );

                if (response != null)
                {
                    AllPromoCodes = response.Result;
                    TotalPages = response.PageCount;
                    TotalCount = response.TotalCount;
                    FilteredCount = response.FilteredCount;
                }
                else
                {
                    ErrorMessage = "Erreur lors du chargement des codes promos.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Change la page
        /// </summary>
        public async Task ChangePageAsync(int newPage)
        {
            if (newPage >= 1 && newPage <= TotalPages)
            {
                CurrentPage = newPage;
                await LoadPromoCodesAsync();
            }
        }

        /// <summary>
        /// Change la taille de page
        /// </summary>
        public async Task ChangePageSizeAsync(int newPageSize)
        {
            PageSize = newPageSize;
            CurrentPage = 1; // Reset à la première page
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Change le tri
        /// </summary>
        public async Task ChangeSortAsync(string? sortKey, string sortType = "asc")
        {
            SortKey = sortKey;
            SortType = sortType;
            CurrentPage = 1; // Reset à la première page
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Effectue une recherche
        /// </summary>
        public async Task SearchAsync()
        {
            CurrentPage = 1; // Reset à la première page lors d'une recherche
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Ajoute ou modifie un filtre
        /// </summary>
        public async Task AddFilterAsync(string filterKey, List<string> filterValues)
        {
            Filters[filterKey] = filterValues;
            CurrentPage = 1; // Reset à la première page
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Supprime un filtre
        /// </summary>
        public async Task RemoveFilterAsync(string filterKey)
        {
            Filters.Remove(filterKey);
            CurrentPage = 1; // Reset à la première page
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Réinitialise tous les filtres
        /// </summary>
        public async Task ResetFiltersAsync()
        {
            SearchQuery = string.Empty;
            Filters.Clear();
            SortKey = null;
            SortType = "asc";
            CurrentPage = 1;
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Ouvre le modal de création
        /// </summary>
        public void OpenCreateModal()
        {
            EditingPromoCode = new PromoCodeDTO
            {
                ValidityStart = DateTime.Now,
                Code = string.Empty
            };
            ShowCreateModal = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        /// <summary>
        /// Ouvre le modal d'édition
        /// </summary>
        public void OpenEditModal(PromoCodeDTO promoCode)
        {
            EditingPromoCode = new PromoCodeDTO
            {
                PromoCodeId = promoCode.PromoCodeId,
                Code = promoCode.Code,
                RemainingUses = promoCode.RemainingUses,
                DiscountPercentage = promoCode.DiscountPercentage,
                DiscountAmount = promoCode.DiscountAmount,
                ValidityStart = promoCode.ValidityStart,
                ExpiryDate = promoCode.ExpiryDate,
                RefreshDelay = promoCode.RefreshDelay,
                CaseId = promoCode.CaseId,
                UserId = promoCode.UserId
            };
            ShowEditModal = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;
        }

        /// <summary>
        /// Crée un nouveau code promo
        /// </summary>
        public async Task CreatePromoCodeAsync()
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(EditingPromoCode.Code))
                {
                    ErrorMessage = "Le code est obligatoire.";
                    IsProcessing = false;
                    return;
                }

                if (!EditingPromoCode.DiscountPercentage.HasValue && !EditingPromoCode.DiscountAmount.HasValue)
                {
                    ErrorMessage = "Vous devez spécifier au moins un pourcentage ou un montant de réduction.";
                    IsProcessing = false;
                    return;
                }

                bool success = await _promoCodeService.CreatePromoCodeAsync(EditingPromoCode);
                
                if (success)
                {
                    SuccessMessage = "Code promo créé avec succès.";
                    ShowCreateModal = false;
                    await LoadPromoCodesAsync();
                }
                else
                {
                    ErrorMessage = "Erreur lors de la création du code promo.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Met à jour un code promo existant
        /// </summary>
        public async Task UpdatePromoCodeAsync()
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                if (string.IsNullOrWhiteSpace(EditingPromoCode.Code))
                {
                    ErrorMessage = "Le code est obligatoire.";
                    IsProcessing = false;
                    return;
                }

                bool success = await _promoCodeService.UpdatePromoCodeAsync(EditingPromoCode);
                
                if (success)
                {
                    SuccessMessage = "Code promo mis à jour avec succès.";
                    ShowEditModal = false;
                    await LoadPromoCodesAsync();
                }
                else
                {
                    ErrorMessage = "Erreur lors de la mise à jour du code promo.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Supprime un code promo
        /// </summary>
        public async Task DeletePromoCodeAsync(int promoCodeId)
        {
            try
            {
                IsProcessing = true;
                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

                bool success = await _promoCodeService.DeletePromoCodeAsync(promoCodeId);
                
                if (success)
                {
                    SuccessMessage = "Code promo supprimé avec succès.";
                    await LoadPromoCodesAsync();
                }
                else
                {
                    ErrorMessage = "Erreur lors de la suppression du code promo.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur : {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        /// <summary>
        /// Ferme les modaux
        /// </summary>
        public void CloseModals()
        {
            ShowCreateModal = false;
            ShowEditModal = false;
            ErrorMessage = string.Empty;
        }
    }
}
