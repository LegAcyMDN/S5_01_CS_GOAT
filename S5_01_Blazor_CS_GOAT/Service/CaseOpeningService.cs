using System.Net.Http.Headers;
using System.Net.Http.Json;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_Blazor_CS_GOAT.Service
{
    /// <summary>
    /// Service dédié à l'ouverture des cases
    /// Respecte le principe de responsabilité unique (SRP)
    /// </summary>
    public class CaseOpeningService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthService _authService;
        private readonly IService<CaseDTO> _caseRepository;

        public CaseOpeningService(
            HttpClient httpClient,
            AuthService authService,
            IService<CaseDTO> caseRepository)
        {
            _httpClient = httpClient;
            _authService = authService;
            _caseRepository = caseRepository;
        }

        /// <summary>
        /// Ouvre une ou plusieurs cases
        /// </summary>
        public async Task<MultipleCaseResultDTO> OpenCasesAsync(int caseId, int quantity, string? promoCode = null)
        {
            var caseOpeningInfo = new CaseOpenningDTO
            {
                CaseId = caseId,
                Quantity = quantity,
                RaffleRollerLength = 82,
                PromoCode = string.IsNullOrEmpty(promoCode) ? null : promoCode
            };

            string jwtToken = await _authService.GetTokenAsync();
            MultipleCaseResultDTO casesReturn = await _caseRepository.OpenCaseAsync(caseOpeningInfo, jwtToken);

            // Rafraîchir le portefeuille de l'utilisateur
            await _authService.LoadCurrentUserAsync();
            _authService.NotifyUserDataChanged();

            return casesReturn;
        }
    }
}
