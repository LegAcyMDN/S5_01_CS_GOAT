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

        // DTO → Entity
        CreateMap<InventoryItemDTO, InventoryItem>()
            .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
            .ForMember(dest => dest.Float, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.WearId, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedOn, opt => opt.Ignore())
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
            .ForMember(dest => dest.AcquiredOn, opt => opt.MapFrom(src => src.AcquiredOn))
            .ForMember(dest => dest.Wear, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.UpgradeResults, opt => opt.Ignore())
            .ForMember(dest => dest.ItemTransactions, opt => opt.Ignore());
    }
}