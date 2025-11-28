using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class NotificationTypeMapper : Profile
    {
        public NotificationTypeMapper()
        {
            // Entity -> DTO
            CreateMap<NotificationType, NotificationTypeDTO>()
                .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationTypeName));

            // DTO -> Entity
            CreateMap<NotificationTypeDTO, NotificationType>()
                .ForMember(dest => dest.NotificationTypeId, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationTypeName))
                .ForMember(dest => dest.NotificationSettings, opt => opt.Ignore())
                .ForMember(dest => dest.Notifications, opt => opt.Ignore());
        }
    }
}
