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

        public async Task<MultipleCaseResultDTO> OpenCasesAsync(int caseId, int quantity, string? promoCode = null)
        {
            CaseOpenningDTO caseOpeningInfo = new CaseOpenningDTO
            {
                CaseId = caseId,
                Quantity = quantity,
                RaffleRollerLength = 82,
                PromoCode = string.IsNullOrEmpty(promoCode) ? null : promoCode
            };

            string jwtToken = await _authService.GetTokenAsync();
            MultipleCaseResultDTO casesReturn = await _caseRepository.OpenCaseAsync(caseOpeningInfo, jwtToken);

            await _authService.LoadCurrentUserAsync();
            _authService.NotifyUserDataChanged();

            return casesReturn;
        }

        public async Task<double> PreviewPriceAsync(int caseId, int quantity, string? promoCode = null)
        {
            
            if (string.IsNullOrEmpty(promoCode))
            {
                CaseDTO caseInfo = await _caseRepository.GetByIdAsync(caseId, await _authService.GetTokenAsync());
                return caseInfo.CasePrice * quantity;
            }

            string jwtToken = await _authService.GetTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

            HttpResponseMessage response = await _httpClient.GetAsync($"PromoCode/check/{promoCode}?caseId={caseId}");
            
            if (!response.IsSuccessStatusCode)
            {
                throw new Shared.Exceptions.CaseExceptions.InvalidPromoCodeException("Code promo invalide");
            }

            CasePromoCodeDTO? promoCodeInfo = await response.Content.ReadFromJsonAsync<CasePromoCodeDTO>();
            
            if (promoCodeInfo == null || promoCodeInfo.FinalPrice == null)
            {
                throw new Shared.Exceptions.CaseExceptions.InvalidPromoCodeException("Code promo invalide");
            }

            return promoCodeInfo.FinalPrice.Value * quantity;
        }
    }
}
