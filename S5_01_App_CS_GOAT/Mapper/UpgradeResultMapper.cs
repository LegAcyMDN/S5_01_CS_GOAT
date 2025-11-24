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
                .ForMember(dest => dest.DegradeFunction, opt => opt.MapFrom(src => src.DegradeFunction))
                .ForMember(dest => dest.UserIdUpgrade, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.WearIdUpgrade, opt => opt.MapFrom(src => src.WearId))
                .ForMember(dest => dest.FairRandomId, opt => opt.MapFrom(src => src.FairRandomId))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.RandomTransaction.TransactionId));

            // DTO -> Entity 
            CreateMap<UpgradeResultDTO, UpgradeResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserIdUpgrade))
                .ForMember(dest => dest.WearId, opt => opt.MapFrom(src => src.WearIdUpgrade))
                .ForMember(dest => dest.FloatStart, opt => opt.MapFrom(src => src.FloatStart))
                .ForMember(dest => dest.FloatEnd, opt => opt.MapFrom(src => src.FloatEnd))
                .ForMember(dest => dest.ProbIntact, opt => opt.MapFrom(src => src.ProbIntact))
                .ForMember(dest => dest.ProbDegrade, opt => opt.MapFrom(src => src.ProbDegrade))
                .ForMember(dest => dest.PropDestroy, opt => opt.MapFrom(src => src.PropDestroy))
                .ForMember(dest => dest.DegradeFunction, opt => opt.MapFrom(src => src.DegradeFunction))
                .ForMember(dest => dest.FairRandomId, opt => opt.MapFrom(src => src.FairRandomId))
                .ForMember(dest => dest.RandomTransaction, opt => opt.Ignore());
        }
    }
}
