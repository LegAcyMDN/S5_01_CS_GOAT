using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Mapper;

/// <summary>
/// Mapper pour PromoCode -> PromoCodeDTO
/// </summary>
public class PromoCodeMapper : Profile
{
    public PromoCodeMapper()
    {
        // Entity -> CasePromoCodeDTO (pour l'endpoint check)
        _ = CreateMap<PromoCode, CasePromoCodeDTO>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
            .ForMember(dest => dest.RemainingUses, opt => opt.MapFrom(src => src.RemainingUses))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
            .ForMember(dest => dest.ValidityStart, opt => opt.MapFrom(src => src.ValidityStart))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.NextRefresh, opt => opt.MapFrom(src => src.NextRefresh()));

        // Entity -> PromoCodeDTO (pour l'admin avec GetOptions)
        _ = CreateMap<PromoCode, PromoCodeDTO>()
            .ForMember(dest => dest.PromoCodeId, opt => opt.MapFrom(src => src.PromoCodeId))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.RemainingUses, opt => opt.MapFrom(src => src.RemainingUses))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
            .ForMember(dest => dest.ValidityStart, opt => opt.MapFrom(src => src.ValidityStart))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.RefreshDelay, opt => opt.MapFrom(src => src.RefreshDelay))
            .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.Case != null ? src.Case.CaseName : null))
            .ForMember(dest => dest.UserLogin, opt => opt.MapFrom(src => src.User != null ? src.User.Login : null));

        // PromoCodeDTO -> Entity (pour Create et Update)
        _ = CreateMap<PromoCodeDTO, PromoCode>()
            .ForMember(dest => dest.PromoCodeId, opt => opt.MapFrom(src => src.PromoCodeId))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.RemainingUses, opt => opt.MapFrom(src => src.RemainingUses))
            .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
            .ForMember(dest => dest.DiscountAmount, opt => opt.MapFrom(src => src.DiscountAmount))
            .ForMember(dest => dest.ValidityStart, opt => opt.MapFrom(src => src.ValidityStart))
            .ForMember(dest => dest.ExpiryDate, opt => opt.MapFrom(src => src.ExpiryDate))
            .ForMember(dest => dest.RefreshDelay, opt => opt.MapFrom(src => src.RefreshDelay))
            .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Case, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}
