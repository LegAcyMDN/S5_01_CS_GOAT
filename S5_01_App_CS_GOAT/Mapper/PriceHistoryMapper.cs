using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class PriceHistoryMapper : Profile
{
    public PriceHistoryMapper()
    {
        //// Entity → DTO
        //CreateMap<PriceHistory, PriceHistoryDTO>()
        //    .ForMember(dest => dest.PriceDate, opt => opt.MapFrom(src => src.PriceDate))
        //    .ForMember(dest => dest.PriceValue, opt => opt.MapFrom(src => src.PriceValue));

        //// DTO → Entity
        //CreateMap<PriceHistoryDTO, PriceHistory>()
        //    .ForMember(dest => dest.PriceHistoryId, opt => opt.Ignore())
        //    .ForMember(dest => dest.ItemId, opt => opt.Ignore())
        //    .ForMember(dest => dest.Item, opt => opt.Ignore());
    }
}