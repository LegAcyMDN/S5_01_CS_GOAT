using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class CaseDetailMapper : Profile
    {
        public CaseDetailMapper() 
        {
            // Entity -> DetailDTO
            CreateMap<Case, CaseDetailDTO>()
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.CaseName))
                .ForMember(dest => dest.CaseImage, opt => opt.MapFrom(src => src.CaseImage))
                .ForMember(dest => dest.CasePrice, opt => opt.MapFrom(src => src.CasePrice))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.CaseContents != null && src.CaseContents.Any() ? src.CaseContents.Sum(cc => cc.Weight) : 0))
                .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => false)); // À modifier plus tard pour le JWT
        }
    }
}
