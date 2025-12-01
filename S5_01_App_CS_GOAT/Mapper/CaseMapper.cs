using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class CaseMapper : Profile
    {
        public CaseMapper()
        {
            // Entity -> DetailDTO
            CreateMap<Case, CaseDTO>()
                .ForMember(dest => dest.CaseId, opt => opt.MapFrom(src => src.CaseId))
                .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.CaseName))
                .ForMember(dest => dest.CaseImage, opt => opt.MapFrom(src => src.CaseImage))
                .ForMember(dest => dest.CasePrice, opt => opt.MapFrom(src => src.CasePrice))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src =>
                    src.CaseContents == null ? (int?)null : src.CaseContents.Sum(cc => cc.Weight)))
                .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => false));
        }
    }
}
