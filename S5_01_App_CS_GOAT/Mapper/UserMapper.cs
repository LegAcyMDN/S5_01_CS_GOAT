using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using S5_01_App_CS_GOAT.Models.Partials;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            // Entity -> DTO
            CreateMap<User, UserDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
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
        }
    }
}
