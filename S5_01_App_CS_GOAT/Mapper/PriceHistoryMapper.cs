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

            CreateMap<PriceHistoryDTO, PriceHistory>()
                .ForMember(dest => dest.PriceHistoryId, opt => opt.Ignore())
                .ForMember(dest => dest.WearId, opt => opt.Ignore())
                .ForMember(dest => dest.Wear, opt => opt.Ignore())
                .ForMember(dest => dest.GuessDate, opt => opt.MapFrom(src => src.IsGuess ? DateTime.UtcNow : (DateTime?)null))
                .ForMember(dest => dest.PriceDate, opt => opt.MapFrom(src => src.PriceDate))
                .ForMember(dest => dest.PriceValue, opt => opt.MapFrom(src => src.PriceValue));
        }
    }
}
