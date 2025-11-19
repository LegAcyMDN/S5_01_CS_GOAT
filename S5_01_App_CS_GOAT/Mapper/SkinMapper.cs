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

        //DTO → Entity
        CreateMap<SkinDTO, Skin>()
            .ForMember(dest => dest.SkinId, opt => opt.Ignore())
            .ForMember(dest => dest.ItemId, opt => opt.Ignore())
            .ForMember(dest => dest.PaintIndex, opt => opt.Ignore())
            .ForMember(dest => dest.RarityId, opt => opt.Ignore());

        //Entity → DetailDTO
        CreateMap<Skin, SkinDetailDTO>()
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.SkinName))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.ItemName))
            .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.Item.ItemType.ItemTypeName))
            .ForMember(dest => dest.RarityName, opt => opt.MapFrom(src => src.Rarity.RarityName))
            .ForMember(dest => dest.WearName, opt => opt.MapFrom(src => src.Wears.First().WearName))
            .ForMember(dest => dest.FloatLow, opt => opt.MapFrom(src => src.Wears.First().FloatLow))
            .ForMember(dest => dest.FloatHigh, opt => opt.MapFrom(src => src.Wears.First().FloatHigh))
            .ForMember(dest => dest.Uuid, opt => opt.MapFrom(src => src.Wears.First().Uuid));

        //DetailDTO → Entity
        CreateMap<SkinDetailDTO, Skin>()
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.SkinName))
            .ForMember(dest => dest.Item, opt => opt.Ignore())
            .ForMember(dest => dest.ItemId, opt => opt.Ignore())
            .ForMember(dest => dest.Item.ItemTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.Item.ItemType.ItemTypeName, opt => opt.MapFrom(src => src.ItemTypeName))
            .ForMember(dest => dest.Item.ItemType.ParentItemTypeId, opt => opt.Ignore())
            .ForMember(dest => dest.Rarity, opt => opt.Ignore())
            .ForMember(dest => dest.RarityId, opt => opt.Ignore())
            .ForMember(dest => dest.PaintIndex, opt => opt.Ignore())
            .ForMember(dest => dest.ItemId, opt => opt.Ignore())
            .ForMember(dest => dest.Item, opt => opt.Ignore())
            .ForMember(dest => dest.CaseContents, opt => opt.Ignore())

            // Reconstruction d'une liste Wear
            .ForMember(dest => dest.Wears, opt => opt.MapFrom(src =>
                new List<Wear>
                {
                    new Wear
                    {
                        WearName = src.WearName,
                        FloatLow = src.FloatLow,
                        FloatHigh = src.FloatHigh,
                        Uuid = src.Uuid
                    }
                }));
    }
}