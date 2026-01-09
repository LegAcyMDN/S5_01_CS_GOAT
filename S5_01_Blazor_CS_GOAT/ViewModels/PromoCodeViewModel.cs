using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la gestion des codes promos dans l'admin
    /// </summary>
    public class PromoCodeViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        private List<PromoCode>? _allPromoCodes;
        private PromoCode? _selectedPromoCode;
        private PromoCode _editingPromoCode = new PromoCode();
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private string _errorMessage = string.Empty;
        private string _successMessage = string.Empty;
        private string _searchQuery = string.Empty;
        private bool _showCreateModal = false;
        private bool _showEditModal = false;
        
        // Filtres
        private string _filterType = "all"; // all, user, case, global
        private DateTime? _filterDateFrom;
        private DateTime? _filterDateTo;
        private string _sortBy = "code"; // code, validityStart, expiryDate, discountPercentage, discountAmount

        public PromoCodeViewModel(AuthService authService, HttpClient httpClient)
        {
            _authService = authService;
            _httpClient = httpClient;
        }

        #region Properties

        public List<PromoCode>? AllPromoCodes
        {
            get => _allPromoCodes;
            set => SetProperty(ref _allPromoCodes, value);
        }

        public PromoCode? SelectedPromoCode
        {
            get => _selectedPromoCode;
            set => SetProperty(ref _selectedPromoCode, value);
        }

        public PromoCode EditingPromoCode
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

        public string FilterType
        {
            get => _filterType;
            set => SetProperty(ref _filterType, value);
        }

        public DateTime? FilterDateFrom
        {
            get => _filterDateFrom;
            set => SetProperty(ref _filterDateFrom, value);
        }

        public DateTime? FilterDateTo
        {
            get => _filterDateTo;
            set => SetProperty(ref _filterDateTo, value);
        }

        public string SortBy
        {
            get => _sortBy;
            set => SetProperty(ref _sortBy, value);
        }

        /// <summary>
        /// Calcule la prochaine date de rafraîchissement pour un code promo
        /// </summary>
        public DateTime? GetNextRefresh(PromoCode promo)
        {
            if (promo.RefreshDelay == null || promo.ExpiryDate == null) return null;
            return promo.ExpiryDate.Value.Add(promo.RefreshDelay.Value);
        }

        /// <summary>
        /// Vérifie si un code promo est expiré
        /// </summary>
        public bool IsExpired(PromoCode promo)
        {
            return promo.ExpiryDate.HasValue && DateTime.Now > promo.ExpiryDate.Value;
        }

        /// <summary>
        /// Vérifie si un code promo est prêt à être rafraîchi
        /// </summary>
        public bool IsDueForRefresh(PromoCode promo)
        {
            if (!IsExpired(promo)) return false;
            if (promo.RefreshDelay == null) return false;
            var nextRefresh = GetNextRefresh(promo);
            return nextRefresh.HasValue && DateTime.Now >= nextRefresh.Value;
        }

        /// <summary>
        /// Vérifie si un code promo sera automatiquement supprimé
        /// </summary>
        public bool WillBeDeleted(PromoCode promo)
        {
            if (promo.RefreshDelay != null) return false;
            return IsExpired(promo) || promo.RemainingUses == 0;
        }

        /// <summary>
        /// Obtient le statut d'un code promo avec sa description
        /// </summary>
        public (string Status, string Description, string CssClass) GetPromoStatus(PromoCode promo)
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

        public List<PromoCode> FilteredAndSortedPromoCodes
        {
            get
            {
                if (AllPromoCodes == null) return new List<PromoCode>();

                var filtered = AllPromoCodes.AsEnumerable();

                // Filtrer par recherche
                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    filtered = filtered.Where(p =>
                        p.Code.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        (p.UserLogin != null && p.UserLogin.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)) ||
                        (p.CaseName != null && p.CaseName.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase))
                    );
                }

                // Filtrer par type
                if (FilterType != "all")
                {
                    filtered = FilterType switch
                    {
                        "user" => filtered.Where(p => p.UserId != null),
                        "case" => filtered.Where(p => p.CaseId != null),
                        "global" => filtered.Where(p => p.UserId == null && p.CaseId == null),
                        _ => filtered
                    };
                }

                // Filtrer par date
                if (FilterDateFrom.HasValue)
                {
                    filtered = filtered.Where(p => p.ValidityStart >= FilterDateFrom.Value);
                }
                if (FilterDateTo.HasValue)
                {
                    filtered = filtered.Where(p => !p.ExpiryDate.HasValue || p.ExpiryDate.Value <= FilterDateTo.Value);
                }

                // Trier
                filtered = SortBy switch
                {
                    "validityStart" => filtered.OrderBy(p => p.ValidityStart),
                    "expiryDate" => filtered.OrderBy(p => p.ExpiryDate ?? DateTime.MaxValue),
                    "discountPercentage" => filtered.OrderByDescending(p => p.DiscountPercentage ?? 0),
                    "discountAmount" => filtered.OrderByDescending(p => p.DiscountAmount ?? 0),
                    _ => filtered.OrderBy(p => p.Code)
                };

                return filtered.ToList();
            }
        }

        #endregion

        public override async Task InitializeAsync()
        {
            await LoadPromoCodesAsync();
        }

        /// <summary>
        /// Charge tous les codes promos
        /// </summary>
        public async Task LoadPromoCodesAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("PromoCode/all");
                if (response.IsSuccessStatusCode)
                {
                    AllPromoCodes = await response.Content.ReadFromJsonAsync<List<PromoCode>>();
                }
                else
                {
                    ErrorMessage = $"Erreur lors du chargement des codes promos : {response.StatusCode}";
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
        /// Ouvre le modal de création
        /// </summary>
        public void OpenCreateModal()
        {
            EditingPromoCode = new PromoCode
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
        public void OpenEditModal(PromoCode promoCode)
        {
            EditingPromoCode = new PromoCode
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

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("PromoCode/create", EditingPromoCode);
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Code promo créé avec succès.";
                    ShowCreateModal = false;
                    await LoadPromoCodesAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Erreur lors de la création : {errorContent}";
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

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PutAsJsonAsync($"PromoCode/update/{EditingPromoCode.PromoCodeId}", EditingPromoCode);
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Code promo mis à jour avec succès.";
                    ShowEditModal = false;
                    await LoadPromoCodesAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Erreur lors de la mise à jour : {errorContent}";
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

                var token = await _authService.GetTokenAsync();
                if (string.IsNullOrEmpty(token)) return;

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.DeleteAsync($"PromoCode/delete/{promoCodeId}");
                if (response.IsSuccessStatusCode)
                {
                    SuccessMessage = "Code promo supprimé avec succès.";
                    await LoadPromoCodesAsync();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ErrorMessage = $"Erreur lors de la suppression : {errorContent}";
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

        /// <summary>
        /// Réinitialise les filtres
        /// </summary>
        public void ResetFilters()
        {
            SearchQuery = string.Empty;
            FilterType = "all";
            FilterDateFrom = null;
            FilterDateTo = null;
            SortBy = "code";
        }
    }
}
