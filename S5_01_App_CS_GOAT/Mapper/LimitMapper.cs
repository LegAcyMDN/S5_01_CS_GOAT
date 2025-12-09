using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class LimitMapper : Profile
    {
        public LimitMapper()
        {
            // Entity -> DTO
            CreateMap<Limit, LimitDTO>()
                .ForMember(dest => dest.LimitAmount, opt => opt.MapFrom(src => src.LimitAmount))
                .ForMember(dest => dest.LimitTypeName, opt => opt.MapFrom(src => src.LimitType.LimitTypeName));
        }
    }
}
