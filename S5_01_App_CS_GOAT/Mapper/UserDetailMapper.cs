using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Partials;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class UserDetailMapper : Profile
    {
        public UserDetailMapper()
        {
            // Entity -> DTO
            CreateMap<User, UserDetailDTO>()
                .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.Login))
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PhoneIsVerified, opt => opt.MapFrom(src => src.PhoneVerifiedOn.HasValue))
                .ForMember(dest => dest.EmailIsVerified, opt => opt.MapFrom(src => src.EmailVerifiedOn.HasValue))
                .ForMember(dest => dest.TwoFA, opt => opt.MapFrom(src => 
                    src.TwoFaIsPhone ? TwoFAmethod.Phone : 
                    src.TwoFaIsEmail ? TwoFAmethod.Email : 
                    TwoFAmethod.None))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.Seed, opt => opt.MapFrom(src => src.Seed))
                .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
                .ForMember(dest => dest.IsSteamLogin, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.SteamId)));

            // DTO -> Entity
            CreateMap<UserDetailDTO, User>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.Login, opt => opt.Ignore())
                .ForMember(dest => dest.SaltPassword, opt => opt.Ignore())
                .ForMember(dest => dest.HashPassword, opt => opt.Ignore())
                .ForMember(dest => dest.SteamId, opt => opt.Ignore())
                .ForMember(dest => dest.Limits, opt => opt.Ignore())
                .ForMember(dest => dest.Tokens, opt => opt.Ignore())
                .ForMember(dest => dest.UserNotifications, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationSettings, opt => opt.Ignore())
                .ForMember(dest => dest.Transactions, opt => opt.Ignore())
                .ForMember(dest => dest.Bans, opt => opt.Ignore())
                .ForMember(dest => dest.Favorites, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItems, opt => opt.Ignore())
                .ForMember(dest => dest.PromoCodes, opt => opt.Ignore())
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.PhoneVerifiedOn, opt => opt.Ignore())
                .ForMember(dest => dest.EmailVerifiedOn, opt => opt.Ignore())
                .ForMember(dest => dest.TwoFaIsPhone, opt => opt.MapFrom(src => src.TwoFA == TwoFAmethod.Phone))
                .ForMember(dest => dest.TwoFaIsEmail, opt => opt.MapFrom(src => src.TwoFA == TwoFAmethod.Email))
                .ForMember(dest => dest.IsAdmin, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.LastLogin, opt => opt.Ignore())
                .ForMember(dest => dest.Seed, opt => opt.Ignore())
                .ForMember(dest => dest.Nonce, opt => opt.Ignore())
                .ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
                .ForMember(dest => dest.DeleteOn, opt => opt.Ignore());
        }
    }
}
