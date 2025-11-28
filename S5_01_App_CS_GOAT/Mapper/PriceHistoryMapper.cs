using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class PriceHistoryMapper : Profile
    {
        public PriceHistoryMapper()
        {
            CreateMap<PriceHistory, PriceHistoryDTO>()
                .ForMember(dest => dest.PriceDate, opt => opt.MapFrom(src => src.PriceDate))
                .ForMember(dest => dest.PriceValue, opt => opt.MapFrom(src => src.PriceValue))
                .ForMember(dest => dest.IsGuess, opt => opt.MapFrom(src => src.GuessDate.HasValue));
        }
    }
}
