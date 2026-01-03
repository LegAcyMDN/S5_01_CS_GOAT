using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class TokenMapper : Profile
    {
        public TokenMapper()
        {
            // Entity -> DTO
            CreateMap<Token, TokenDTO>()
                .ForMember(dest => dest.TokenId, opt => opt.MapFrom(src => src.TokenId))
                .ForMember(dest => dest.TokenValue, opt => opt.MapFrom(src => src.TokenValue))
                .ForMember(dest => dest.TokenExpiry, opt => opt.MapFrom(src => src.TokenExpiry))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
        }
    }
}
