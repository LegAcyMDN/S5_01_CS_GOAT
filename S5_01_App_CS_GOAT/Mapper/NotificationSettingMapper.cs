using AutoMapper;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class NotificationSettingMapper : Profile
    {
        public NotificationSettingMapper()
        {
            // Entity → DTO
            CreateMap<NotificationSetting, NotificationSettingDTO>()
                .ForMember(dest => dest.OnSite, opt => opt.MapFrom(src => src.OnSite))
                .ForMember(dest => dest.ByEmail, opt => opt.MapFrom(src => src.ByEmail))
                .ForMember(dest => dest.ByPhone, opt => opt.MapFrom(src => src.ByPhone))
                .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationType.NotificationTypeName));

        }
    }
}
