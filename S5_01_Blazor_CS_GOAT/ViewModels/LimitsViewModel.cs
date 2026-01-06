using S5_01_Blazor_CS_GOAT.Models;
using S5_01_Blazor_CS_GOAT.Service;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace S5_01_Blazor_CS_GOAT.ViewModels
{
    /// <summary>
    /// ViewModel pour la page Limits - Gère les limites
    /// </summary>
    public class LimitsViewModel : ViewModelBase
    {
        private readonly IService<Limit> _limitRepository;
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;
        private readonly NavigationManager  _navigationManager;
        private User? _currentUser;
        private List<Limit>? _limits;
        private List<Limit>? _sortedLimits;
        private string _type = "";
        private string _period = "";
        private double? _amount = null;
        private bool _isLoading = true;

        public LimitsViewModel(
            IService<Limit> limitRepository,
            AuthService authService,
            HttpClient httpClient, 
            NavigationManager navigationManager)
        {
            _limitRepository = limitRepository;
            _authService = authService;
            _httpClient = httpClient;
            _navigationManager = navigationManager;
        }
        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }
        public List<Limit>? Limits
        {
            get => _limits;
            set => SetProperty(ref _limits, value);
        }
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public List<Limit>? SortedLimits
        {
            get => _sortedLimits;
            set => SetProperty(ref _sortedLimits, value);
        }

        public string Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string Period
        {
            get => _period;
            set => SetProperty(ref _period, value);
        }

        public double? Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value);
        }

        public override async Task InitializeAsync()
        {
            await LoadLimitsDataAsync();
        }

        /// <summary>
        /// Charge toutes les données du portefeuille
        /// </summary>
        private async Task LoadLimitsDataAsync()
        {
            try
            {
                IsLoading = true;

                await UpdateUserStateAsync();

                string jwtToken = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    Limits = await _limitRepository.GetByUserAsync(jwtToken);
                    SortLimit(Period, Type);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des limites: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        /// <summary>
        /// Met à jour l'état de l'utilisateur
        /// </summary>
        private async Task UpdateUserStateAsync()
        {
            if (_authService.CurrentUser == null && await _authService.IsLoggedInAsync())
            {
                await _authService.LoadCurrentUserAsync();
            }

            CurrentUser = _authService.CurrentUser;
        }

        /// <summary>
        /// Sauvegarde une limite de budget
        /// </summary>
        public async Task SaveLimitAsync()
        {
            if (Limits == null)
            {
                Console.WriteLine("Aucune limite chargée.");
                return;
            }

            if (string.IsNullOrEmpty(Type))
            {
                Console.WriteLine("Sélectionne un type avant de sauvegarder.");
                return;
            }

            if (string.IsNullOrEmpty(Period))
            {
                Console.WriteLine("Sélectionne une période avant de sauvegarder.");
                return;
            }

            var limitTypeName = $"{Type} {Period}";

            var token = await _authService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Utilisateur non authentifié.");
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                LimitAmount = Amount,
                LimitTypeName = limitTypeName
            };
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"Limit/update")
            {
                Content = JsonContent.Create(payload)
            };

            var resp = await _httpClient.SendAsync(request);
            if (resp.IsSuccessStatusCode)
            {
                var jwtToken = await _authService.GetTokenAsync();
                Limits = await _limitRepository.GetByUserAsync(jwtToken);
                SortLimit(Period, Type);
            }
            else
            {
                var err = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur sauvegarde limite: {err}");
            }
        }

        /// <summary>
        /// Affiche les limites correspondantes a la selection
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public List<Limit> SortLimit(string? period, string? type)
        {
            SortedLimits = new List<Limit>();
            Period = period;
            Type = type;

            foreach (var l in Limits)
            {
                bool containPeriod = l.LimitTypeName.Contains(period);
                bool containType = l.LimitTypeName.Contains(type);

                if (!string.IsNullOrEmpty(Period) && !string.IsNullOrEmpty(Type))
                {
                    if (containPeriod && containType)
                        SortedLimits.Add(l);
                    continue;
                }

                if (string.IsNullOrEmpty(Period) && !string.IsNullOrEmpty(Type))
                {
                    if (containType)
                        SortedLimits.Add(l);
                    continue;
                }

                if (!string.IsNullOrEmpty(Period) && string.IsNullOrEmpty(Type))
                {
                    if (containPeriod)
                        SortedLimits.Add(l);
                    continue;
                }

                SortedLimits.Add(l);
            }
            return SortedLimits;
        }
    }
}
