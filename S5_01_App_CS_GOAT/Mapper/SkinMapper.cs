using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class SkinMapper : Profile
{
    public SkinMapper()
    {
        //Entity → DTO
        CreateMap<Skin, SkinDTO>()
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.SkinName));

        // DTO → Entity
        CreateMap<SkinDTO, Skin>()
            .ForMember(dest => dest.SkinId, opt => opt.Ignore())
            .ForMember(dest => dest.ItemId, opt => opt.Ignore())
            .ForMember(dest => dest.PaintIndex, opt => opt.Ignore())
            .ForMember(dest => dest.RarityId, opt => opt.Ignore());
    }
}