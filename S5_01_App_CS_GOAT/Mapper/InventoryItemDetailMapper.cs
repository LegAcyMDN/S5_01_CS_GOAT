using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

public class InventoryItemDetailMapper : Profile
{
    public InventoryItemDetailMapper()
    {
        // Entity -> DTO
        CreateMap<InventoryItem, InventoryItemDetailDTO>()
            .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
            .ForMember(dest => dest.Float, opt => opt.MapFrom(src => src.Float))
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
            .ForMember(dest => dest.AcquiredOn, opt => opt.MapFrom(src => src.AcquiredOn))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Wear.Uuid))
            .ForMember(dest => dest.WearName, opt => opt.MapFrom(src => src.Wear.WearName))
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.Wear.Skin.SkinName))
            .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.Wear.Skin.Rarity.RarityColor))
            .ForMember(dest => dest.RarityName, opt => opt.MapFrom(src => src.Wear.Skin.Rarity.RarityName))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Wear.Skin.Item.ItemName))
            .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.Wear.Skin.Item.ItemType.ItemTypeName));

        // DTO -> Entity
        CreateMap<InventoryItemDetailDTO, InventoryItem>()
            .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.WearId, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedOn, opt => opt.Ignore())
            .ForMember(dest => dest.Float, opt => opt.MapFrom(src => src.Float)) 
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
            .ForMember(dest => dest.AcquiredOn, opt => opt.MapFrom(src => src.AcquiredOn))
            .ForMember(dest => dest.Wear, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.UpgradeResults, opt => opt.Ignore())
            .ForMember(dest => dest.ItemTransactions, opt => opt.Ignore());
    }
}