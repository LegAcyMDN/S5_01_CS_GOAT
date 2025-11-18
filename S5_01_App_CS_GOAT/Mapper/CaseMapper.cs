using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class CaseMapper : Profile
    {
        public CaseMapper()
        {
            // Case -> CaseDto
            CreateMap<Case, CaseDTO>()
                .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.CaseName))
                .ForMember(dest => dest.CaseImage, opt => opt.MapFrom(src => src.CaseImage))
                .ForMember(dest => dest.CasePrice, opt => opt.MapFrom(src => src.CasePrice));

            // CaseDto -> Case
            CreateMap<CaseDTO, Case>()
                .ForMember(dest => dest.CaseId, opt => opt.Ignore()) // On ignore la navigation
                .ForMember(dest => dest.CaseName, opt => opt.MapFrom(src => src.CaseName))
                .ForMember(dest => dest.CaseImage, opt => opt.MapFrom(src => src.CaseImage))
                .ForMember(dest => dest.CasePrice, opt => opt.MapFrom(src => src.CasePrice));
        }
    }
}
