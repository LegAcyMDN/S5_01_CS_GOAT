using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class NotificationTypeMapper : Profile
    {
        public NotificationTypeMapper()
        {
            // Entity -> DTO
            _ = CreateMap<NotificationType, NotificationTypeDTO>()
                .ForMember(dest => dest.NotificationTypeName, opt => opt.MapFrom(src => src.NotificationTypeName));
        }
    }
}
