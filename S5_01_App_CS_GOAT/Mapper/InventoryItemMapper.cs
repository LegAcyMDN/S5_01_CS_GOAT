using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

public class InventoryItemMapper : Profile
{
    public InventoryItemMapper()
    {
        // Entity → DTO
        CreateMap<InventoryItem, InventoryItemDTO>()
            .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
            .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.Wear.Skin.Rarity.RarityColor))
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
            .ForMember(dest => dest.AcquiredOn, opt => opt.MapFrom(src => src.AcquiredOn))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Wear.Uuid));
    }
}