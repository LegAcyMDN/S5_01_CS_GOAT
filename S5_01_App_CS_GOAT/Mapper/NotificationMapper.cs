using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

public class NotificationMapper : Profile
{
    public NotificationMapper()
    {
        // Entity → DTO
        CreateMap<Notification, NotificationDTO>()
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId))
            .ForMember(dest => dest.NotificationSummary, opt => opt.MapFrom(src => src.NotificationSummary))
            .ForMember(dest => dest.NotificationContent, opt => opt.MapFrom(src => src.NotificationContent))
            .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.NotificationDate))
            .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationType.NotificationTypeName));

        // DTO → Entity
        CreateMap<NotificationDTO, Notification>()
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId))
            .ForMember(dest => dest.NotificationSummary, opt => opt.MapFrom(src => src.NotificationSummary))
            .ForMember(dest => dest.NotificationContent, opt => opt.MapFrom(src => src.NotificationContent))
            .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.NotificationDate))
            .ForMember(dest => dest.NotificationTypeId, opt => opt.Ignore())
            .ForPath(dest => dest.NotificationType.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationTypeName));
    }
}
