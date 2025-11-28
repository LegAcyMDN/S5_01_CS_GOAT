using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class ItemTransactionDetailMapper : Profile
    {
        public ItemTransactionDetailMapper()
        {
            // Entity -> DTO
            CreateMap<ItemTransaction, ItemTransactionDetailDTO>()
                .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn))
                .ForMember(dest => dest.WearName, opt => opt.MapFrom(src => src.InventoryItem.Wear.WearName))
                .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.SkinName))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Item.ItemName))
                .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Item.ItemType.ItemTypeName))
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.InventoryItem.Wear.Uuid))
                .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Rarity.RarityColor));
        }
    }
}
