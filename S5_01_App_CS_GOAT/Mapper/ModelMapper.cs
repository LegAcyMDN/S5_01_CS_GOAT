using AutoMapper;
using S5_01_App_CS_GOAT.DTO.Helpers;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

public class ModelMapper : Profile
{
    public ModelMapper()
    {
        // Wear -> DTO
        CreateMap<Wear, ModelDTO>()
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Uuid))
            .ForMember(dest => dest.UvType, opt => opt.MapFrom(src => src.Skin.UvType))
            .ForMember(dest => dest.ItemModel, opt => opt.MapFrom(src => src.Skin.Item.ItemModel));
    }
}
