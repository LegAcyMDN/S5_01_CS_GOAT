using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class ItemMapper : Profile
{
    public ItemMapper()
    {
        //// Entity → DTO
        //CreateMap<Item, ItemDTO>()
        //    .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.ItemName))
        //    .ForMember(dest => dest.ItemModel, opt => opt.MapFrom(src => src.ItemModel))
        //    .ForMember(dest => dest.DefIndex, opt => opt.MapFrom(src => src.DefIndex));

        //// DTO → Entity
        //CreateMap<ItemDTO, Item>()
        //    .ForMember(dest => dest.ItemId, opt => opt.Ignore())
        //    .ForMember(dest => dest.ItemTypeId, opt => opt.Ignore())
        //    .ForMember(dest => dest.ItemType, opt => opt.Ignore())
        //    .ForMember(dest => dest.Skins, opt => opt.Ignore())
        //    .ForMember(dest => dest.PriceHistories, opt => opt.Ignore());
    }
}