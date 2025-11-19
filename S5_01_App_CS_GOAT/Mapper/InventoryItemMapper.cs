using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class InventoryItemMapper : Profile
{
    public InventoryItemMapper()
    {
        //// Entity → DTO
        //CreateMap<InventoryItem, InventoryItemDTO>()
        //    .ForMember(dest => dest.Float, opt => opt.MapFrom(src => src.Float))
        //    .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
        //    .ForMember(dest => dest.AcquiredOn, opt => opt.MapFrom(src => src.AcquiredOn))
        //    .ForMember(dest => dest.RemovedOn, opt => opt.MapFrom(src => src.RemovedOn));

        //// DTO → Entity
        //CreateMap<InventoryItemDTO, InventoryItem>()
        //    .ForMember(dest => dest.InventoryItemId, opt => opt.Ignore())
        //    .ForMember(dest => dest.UserId, opt => opt.Ignore())
        //    .ForMember(dest => dest.WearId, opt => opt.Ignore())
        //    .ForMember(dest => dest.User, opt => opt.Ignore())
        //    .ForMember(dest => dest.Wear, opt => opt.Ignore())
        //    .ForMember(dest => dest.Transactions, opt => opt.Ignore());
    }
}