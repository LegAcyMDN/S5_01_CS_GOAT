using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class RandomTransactionMapper : Profile
    {
        public RandomTransactionMapper()
        {
            // Entity → DTO
            _ = CreateMap<RandomTransaction, RandomTransactionDTO>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn));

            // Entity → DetailDTO
            _ = CreateMap<RandomTransaction, RandomTransactionDetailDTO>()
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

            // Entity → LiveFeedDTO
            _ = CreateMap<RandomTransaction, LiveFeedDTO>()
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Item.ItemName))
                .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.SkinName))
                .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.InventoryItem.Wear.Skin.Rarity.RarityColor))
                .ForMember(dest => dest.WearTypeAbbreviation, opt => opt.MapFrom(src => GetWearTypeAbbreviation(src.InventoryItem.Wear.WearType.WearTypeName)))
                .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.InventoryItem.Wear.Uuid));
        }

        private static string GetWearTypeAbbreviation(string wearTypeName)
        {
            // Extract uppercase letters from the wear type name
            // For example: "FactoryNew" -> "FN", "MinimalWear" -> "MW"
            string abbreviation = string.Concat(wearTypeName.Where(char.IsUpper));
            return string.IsNullOrEmpty(abbreviation) ? wearTypeName : abbreviation;
        }
    }
}
