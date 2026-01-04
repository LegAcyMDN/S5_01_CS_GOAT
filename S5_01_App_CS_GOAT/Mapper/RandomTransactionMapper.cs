using AutoMapper;
using Shared.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class RandomTransactionMapper : Profile
    {
        public RandomTransactionMapper()
        {
            // Entity → DTO
            CreateMap<RandomTransaction, RandomTransactionDTO>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn));

            // Entity → DetailDTO
            CreateMap<RandomTransaction, RandomTransactionDetailDTO>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn))
                .ForMember(dest => dest.WearName, opt => opt.MapFrom(src => src.InventoryItem.Wear.WearType.WearTypeName))
                .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.SkinName))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Item.ItemName))
                .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Item.ItemType.ItemTypeName))
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.InventoryItem.Wear.Uuid))
                .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Rarity.RarityColor))
                .ForMember(dest => dest.Case, opt => opt.MapFrom(src => src.Case));
        }
    }
}
