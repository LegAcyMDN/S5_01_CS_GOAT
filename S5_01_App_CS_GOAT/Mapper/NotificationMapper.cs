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

        // UserNotification → DTO
        CreateMap<UserNotification, NotificationDTO>()
            .IncludeBase<Notification, NotificationDTO>();

        // DTO → UserNotification
        CreateMap<NotificationDTO, UserNotification>()
            .ForMember(dest => dest.NotificationId, opt => opt.MapFrom(src => src.NotificationId))
            .ForMember(dest => dest.NotificationSummary, opt => opt.MapFrom(src => src.NotificationSummary))
            .ForMember(dest => dest.NotificationContent, opt => opt.MapFrom(src => src.NotificationContent))
            .ForMember(dest => dest.NotificationDate, opt => opt.MapFrom(src => src.NotificationDate))
            .ForMember(dest => dest.NotificationType, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => false));
    }
}
