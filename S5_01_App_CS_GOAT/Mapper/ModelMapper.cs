using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
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

        // DTO -> Entity
        CreateMap<ModelDTO, Skin>()
            .ForMember(dest => dest.SkinId, opt => opt.Ignore())
            .ForMember(dest => dest.SkinName, opt => opt.Ignore())
            .ForMember(dest => dest.UvType, opt => opt.MapFrom(src => src.UvType))
            .ForMember(dest => dest.ItemId, opt => opt.Ignore())
            .ForMember(dest => dest.RarityId, opt => opt.Ignore())
            .ForMember(dest => dest.Item, opt => opt.Ignore())
            .ForMember(dest => dest.Rarity, opt => opt.Ignore())
            .ForMember(dest => dest.CaseContents, opt => opt.Ignore())
            .ForMember(dest => dest.Wears, opt => opt.Ignore());
    }
}
