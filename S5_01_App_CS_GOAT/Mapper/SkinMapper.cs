using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper;

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
                    : null))
            .ForMember(dest => dest.Weight, opt => opt.Ignore());

        // CaseContent -> DTO (for case-specific skins with weight)
        CreateMap<CaseContent, SkinDTO>()
            .ForMember(dest => dest.SkinName, opt => opt.MapFrom(src => src.Skin.SkinName))
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Skin.Item.ItemName))
            .ForMember(dest => dest.ItemTypeName, opt => opt.MapFrom(src => src.Skin.Item.ItemType.ItemTypeName))
            .ForMember(dest => dest.RarityName, opt => opt.MapFrom(src => src.Skin.Rarity.RarityName))
            .ForMember(dest => dest.RarityColor, opt => opt.MapFrom(src => src.Skin.Rarity.RarityColor))
            .ForMember(dest => dest.BestPrice, opt => opt.MapFrom(src => 
                src.Skin.Wears != null && src.Skin.Wears.Any() 
                    ? src.Skin.Wears
                        .Where(w => w.PriceHistories != null && w.PriceHistories.Any())
                        .SelectMany(w => w.PriceHistories)
                        .Min(ph => (double?)ph.PriceValue) ?? 0.0
                    : 0.0))
            .ForMember(dest => dest.WorstPrice, opt => opt.MapFrom(src => 
                src.Skin.Wears != null && src.Skin.Wears.Any() 
                    ? src.Skin.Wears
                        .Where(w => w.PriceHistories != null && w.PriceHistories.Any())
                        .SelectMany(w => w.PriceHistories)
                        .Max(ph => (double?)ph.PriceValue) ?? 0.0
                    : 0.0))
            .ForMember(dest => dest.AnyUuid, opt => opt.MapFrom(src => 
                src.Skin.Wears != null && src.Skin.Wears.Any() 
                    ? src.Skin.Wears.FirstOrDefault().Uuid 
                    : null))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight));
    }
}
