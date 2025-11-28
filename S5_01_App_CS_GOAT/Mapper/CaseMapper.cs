using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

public class CaseMapper : Profile
{
    public CaseMapper()
    {
        // Entity -> DTO
        CreateMap<Case, CaseDTO>()
            .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
            .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.CaseName))
            .ForMember(dest => dest.CaseImage, opt => opt.MapFrom(src => src.CaseImage))
            .ForMember(dest => dest.CasePrice, opt => opt.MapFrom(src => src.CasePrice))
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => false));

        // DTO -> Entity
        CreateMap<CaseDTO, Case>()
            .ForMember(dest => dest.CaseId, opt => opt.Ignore())
            .ForMember(dest => dest.CaseContents, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.RandomTransactions, opt => opt.Ignore())
            .ForMember(dest => dest.PromoCodes, opt => opt.Ignore())
            .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.CaseName))
            .ForMember(dest => dest.CaseImage, opt => opt.MapFrom(src => src.CaseImage ?? string.Empty))
            .ForMember(dest => dest.CasePrice, opt => opt.MapFrom(src => src.CasePrice));        
    }
}
