using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class WearMapper : Profile
{
    public WearMapper()
    {
        // Entity → DTO
        //CreateMap<Wear, WearDTO>()
        //    .ForMember(dest => dest.WearValue, opt => opt.MapFrom(src => src.WearValue))
        //    .ForMember(dest => dest.FloatLow, opt => opt.MapFrom(src => src.FloatLow))
        //    .ForMember(dest => dest.FloatHigh, opt => opt.MapFrom(src => src.FloatHigh))
        //    .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid));

        //// DTO → Entity
        //CreateMap<WearDTO, Wear>()
        //    .ForMember(dest => dest.WearId, opt => opt.Ignore())
        //    .ForMember(dest => dest.SkinId, opt => opt.Ignore())
        //    .ForMember(dest => dest.Skin, opt => opt.Ignore())
        //    .ForMember(dest => dest.InventoryItems, opt => opt.Ignore())
        //    .ForMember(dest => dest.UpgradeResults, opt => opt.Ignore());
    }
}