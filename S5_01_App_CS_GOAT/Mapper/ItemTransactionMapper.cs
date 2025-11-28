using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class ItemTransactionMapper : Profile
    {
        public ItemTransactionMapper()
        {
            // Entity → DTO
            CreateMap<ItemTransaction, ItemTransactionDTO>()
                .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn));

            // DTO → Entity
            CreateMap<ItemTransactionDTO, ItemTransaction>()
                .ForMember(dest => dest.InventoryItemId, opt => opt.MapFrom(src => src.InventoryItemId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn))
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.NotificationId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Notification, opt => opt.Ignore())
                .ForMember(dest => dest.InventoryItem, opt => opt.Ignore());
        }
    }
}