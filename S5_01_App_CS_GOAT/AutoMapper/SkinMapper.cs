using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.AutoMapper;

public class SkinMapper : Profile
{
    public SkinMapper()
    {
        // Entity -> DTO
        CreateMap<Skin, SkinDTO>()
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.SkinName))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Item.ItemName))
            .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.Item.ItemType.ItemTypeName))
            .ForMember(dest => dest.RarityName, opt => opt.MapFrom(src => src.Rarity.RarityName))
            .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.Rarity.RarityColor))
            .ForMember(dest => dest.BestPrice, opt => opt.MapFrom(src => 
                src.Wears != null && src.Wears.Any() 
                    ? src.Wears
                        .Where(w => w.PriceHistories != null && w.PriceHistories.Any())
                        .SelectMany(w => w.PriceHistories)
                        .Min(ph => (double?)ph.PriceValue) ?? 0.0
                    : 0.0))
            .ForMember(dest => dest.WorstPrice, opt => opt.MapFrom(src => 
                src.Wears != null && src.Wears.Any() 
                    ? src.Wears
                        .Where(w => w.PriceHistories != null && w.PriceHistories.Any())
                        .SelectMany(w => w.PriceHistories)
                        .Max(ph => (double?)ph.PriceValue) ?? 0.0
                    : 0.0))
            .ForMember(dest => dest.AnyUuid, opt => opt.MapFrom(src => 
                src.Wears != null && src.Wears.Any() 
                    ? src.Wears.FirstOrDefault().Uuid 
                    : null));

        // DTO -> Entity
        CreateMap<SkinDTO, Skin>()
            .ForMember(dest => dest.SkinId, opt => opt.Ignore())
            .ForMember(dest => dest.ItemId, opt => opt.Ignore())
            .ForMember(dest => dest.RarityId, opt => opt.Ignore())
            .ForMember(dest => dest.Item, opt => opt.Ignore())
            .ForMember(dest => dest.Rarity, opt => opt.Ignore())
            .ForMember(dest => dest.CaseContents, opt => opt.Ignore())
            .ForMember(dest => dest.Wears, opt => opt.Ignore())
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.SkinName))
            .ForMember(dest => dest.PaintIndex, opt => opt.Ignore());
    }
}
