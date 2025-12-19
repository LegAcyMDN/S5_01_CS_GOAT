using AutoMapper;
using Shared.DTO;
using Shared.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class PromoCodeMapper : Profile
    {
        public PromoCodeMapper()
        {
            // Entity -> DTO
            CreateMap<PromoCode, CasePromoCodeDTO>()
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.RemainingUses, opt => opt.MapFrom(src => src.RemainingUses))
                .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.ValidityStart, opt => opt.MapFrom(src => src.ValidityStart))
                .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
                .ForMember(dest => dest.NextRefresh, opt => opt.MapFrom(src => src.NextRefresh()));
        }
    }
}
