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
        }
    }
}
