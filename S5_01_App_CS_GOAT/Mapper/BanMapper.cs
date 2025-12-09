using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class BanMapper : Profile
    {
        public BanMapper()
        {
            // Entity -> DTO
            CreateMap<Ban, BanDTO>()
                .ForMember(dest => dest.BanId, opt => opt.MapFrom(src => src.BanId))
                .ForMember(dest => dest.BanReason, opt => opt.MapFrom(src => src.BanReason))
                .ForMember(dest => dest.BanDate, opt => opt.MapFrom(src => src.BanDate))
                .ForMember(dest => dest.BanDuration, opt => opt.MapFrom(src => src.BanDuration))
                .ForMember(dest => dest.BanTypeName, opt => opt.MapFrom(src => src.BanType.BanTypeName))
                .ForMember(dest => dest.BanTypeDescription, opt => opt.MapFrom(src => src.BanType.BanTypeDescription));

            // DTO -> Entity
            CreateMap<BanDTO, Ban>()
                .ForMember(dest => dest.BanId, opt => opt.MapFrom(src => src.BanId))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.BanTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.BanType, opt => opt.Ignore())
                .ForMember(dest => dest.BanReason, opt => opt.MapFrom(src => src.BanReason))
                .ForMember(dest => dest.BanDate, opt => opt.MapFrom(src => src.BanDate))
                .ForMember(dest => dest.BanDuration, opt => opt.MapFrom(src => src.BanDuration));
        }
    }
}
