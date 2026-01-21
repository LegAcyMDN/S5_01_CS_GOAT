using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class PriceHistoryMapper : Profile
    {
        public PriceHistoryMapper()
        {
            _ = CreateMap<PriceHistory, PriceHistoryDTO>()
                .ForMember(dest => dest.PriceDate, opt => opt.MapFrom(src => src.PriceDate))
                .ForMember(dest => dest.PriceValue, opt => opt.MapFrom(src => src.PriceValue))
                .ForMember(dest => dest.IsGuess, opt => opt.MapFrom(src => src.GuessDate.HasValue));
        }
    }
}
