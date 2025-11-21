using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class InventoryItemDetail : Profile
{
    public InventoryItemDetail()
    {
        // Entity → DTO
        CreateMap<InventoryItem, InventoryItemDTO>()
            .ForMember(dest => dest.Float, opt => opt.MapFrom(src => src.Float))
            .ForMember(dest => dest.IsFavorite, opt => opt.MapFrom(src => src.IsFavorite))
            .ForMember(dest => dest.AcquiredOn, opt => opt.MapFrom(src => src.AcquiredOn));

        // DTO → Entity
        CreateMap<InventoryItemDTO, InventoryItem>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.WearId, opt => opt.Ignore())
            .ForMember(dest => dest.RemovedOn, opt => opt.Ignore());
    }
}