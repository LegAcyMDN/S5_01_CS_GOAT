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
                .ForMember(dest => dest.LimitTypeName, opt => opt.MapFrom(src => src.LimitType.LimitTypeName))
                .ForMember(dest => dest.DurationName, opt => opt.MapFrom(src => src.LimitType.DurationName));

            // DTO -> Entity
            CreateMap<LimitDTO, Limit>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.LimitTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.LimitType, opt => opt.Ignore())
                .ForMember(dest => dest.LimitAmount, opt => opt.MapFrom(src => src.LimitAmount));
        }
    }
}
