using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

public class UserMapper : Profile
{
    public UserMapper()
    {
        // Entity → DTO
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.PhoneVerifiedOn, opt => opt.MapFrom(src => src.PhoneVerifiedOn))
            .ForMember(dest => dest.EmailVerifiedOn, opt => opt.MapFrom(src => src.EmailVerifiedOn))
            .ForMember(dest => dest.TwoFaIsPhone, opt => opt.MapFrom(src => src.TwoFaIsPhone))
            .ForMember(dest => dest.TwoFaIsEmail, opt => opt.MapFrom(src => src.TwoFaIsEmail))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
            .ForMember(dest => dest.Seed, opt => opt.MapFrom(src => src.Seed))
            .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet));


        // DTO → Entity
        CreateMap<UserDTO, User>()
            // Champs à ignorer (non présents dans le DTO ou gérés ailleurs)
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.HashPassword, opt => opt.Ignore())
            .ForMember(dest => dest.SaltPassword, opt => opt.Ignore())
            .ForMember(dest => dest.Nonce, opt => opt.Ignore())
            .ForMember(dest => dest.SteamId, opt => opt.Ignore())
            .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
            .ForMember(dest => dest.DeleteOn, opt => opt.Ignore())

            // Collections de navigation → toujours ignorées dans un mapping DTO
            .ForMember(dest => dest.Limits, opt => opt.Ignore())
            .ForMember(dest => dest.Tokens, opt => opt.Ignore())
            .ForMember(dest => dest.UserNotifications, opt => opt.Ignore())
            .ForMember(dest => dest.NotificationSettings, opt => opt.Ignore())
            .ForMember(dest => dest.Transactions, opt => opt.Ignore())
            .ForMember(dest => dest.Bans, opt => opt.Ignore())
            .ForMember(dest => dest.Favorites, opt => opt.Ignore())
            .ForMember(dest => dest.InventoryItems, opt => opt.Ignore());
    }
}
