using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class UpgradeResultMapper : Profile
    {
        public UpgradeResultMapper()
        {
            // Entity -> DTO
            CreateMap<UpgradeResult, UpgradeResultDTO>()
                .ForMember(dest => dest.FloatStart, opt => opt.MapFrom(src => src.FloatStart))
                .ForMember(dest => dest.FloatEnd, opt => opt.MapFrom(src => src.FloatEnd))
                .ForMember(dest => dest.ProbIntact, opt => opt.MapFrom(src => src.ProbIntact))
                .ForMember(dest => dest.ProbDegrade, opt => opt.MapFrom(src => src.ProbDegrade))
                .ForMember(dest => dest.PropDestroy, opt => opt.MapFrom(src => src.PropDestroy))
                .ForMember(dest => dest.DegradeFunction, opt => opt.MapFrom(src => src.DegradeFunction));

            // DTO -> Entity 
            CreateMap<UpgradeResultDTO, UpgradeResult>()
                .ForMember(dest => dest.InventoryItemId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.FairRandomId, opt => opt.Ignore())
                .ForMember(dest => dest.FloatStart, opt => opt.MapFrom(src => src.FloatStart))
                .ForMember(dest => dest.FloatEnd, opt => opt.MapFrom(src => src.FloatEnd))
                .ForMember(dest => dest.ProbIntact, opt => opt.MapFrom(src => src.ProbIntact))
                .ForMember(dest => dest.ProbDegrade, opt => opt.MapFrom(src => src.ProbDegrade))
                .ForMember(dest => dest.PropDestroy, opt => opt.MapFrom(src => src.PropDestroy))
                .ForMember(dest => dest.DegradeFunction, opt => opt.MapFrom(src => src.DegradeFunction))
                .ForMember(dest => dest.InventoryItem, opt => opt.Ignore())
                .ForMember(dest => dest.RandomTransaction, opt => opt.Ignore())
                .ForMember(dest => dest.FairRandom, opt => opt.Ignore());
        }
    }
}
