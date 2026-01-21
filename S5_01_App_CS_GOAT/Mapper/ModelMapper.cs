using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO.Helpers;

namespace S5_01_App_CS_GOAT.Mapper;

public class ModelMapper : Profile
{
    public ModelMapper()
    {
        // Wear -> DTO
        _ = CreateMap<Wear, ModelDTO>()
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.UvType, opt => opt.MapFrom(src => src.Skin.UvType))
            .ForMember(dest => dest.ItemModel, opt => opt.MapFrom(src => src.Skin.Item.ItemModel))
            .ForMember(dest => dest.Texture, opt => opt.MapFrom(src => src.GetTexture().GetAwaiter().GetResult()));
    }
}
