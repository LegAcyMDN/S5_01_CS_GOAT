using AutoMapper;
using S5_01_App_CS_GOAT.DTO;
using S5_01_App_CS_GOAT.Models.EntityFramework;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class FairRandomMapper : Profile
    {
        public FairRandomMapper()
        {
            CreateMap<FairRandom, FairRandomDTO>()
                .ForMember(dest => dest.UserIdUpgrade, opt => opt.MapFrom(src => src.UserIdUpgrade))
                .ForMember(dest => dest.WearIdUpgrade, opt => opt.MapFrom(src => src.WearIdUpgrade))
                .ForMember(dest => dest.ServerSeed, opt => opt.MapFrom(src => src.ServerSeed))
                .ForMember(dest => dest.ServerHash, opt => opt.MapFrom(src => src.ServerHash))
                .ForMember(dest => dest.UserNonce, opt => opt.MapFrom(src => src.UserNonce))
                .ForMember(dest => dest.CombinedHash, opt => opt.MapFrom(src => src.CombinedHash))
                .ForMember(dest => dest.Fraction, opt => opt.MapFrom(src => src.Fraction));

            CreateMap<FairRandomDTO, FairRandom>()
                .ForMember(dest => dest.FairRandomId, opt => opt.Ignore())
                .ForMember(dest => dest.RandomTransactionId, opt => opt.Ignore())
                .ForMember(dest => dest.RandomTransaction, opt => opt.Ignore())
                .ForMember(dest => dest.UpgradeResult, opt => opt.Ignore())
                .ForMember(dest => dest.UserIdUpgrade, opt => opt.MapFrom(src => src.UserIdUpgrade))
                .ForMember(dest => dest.WearIdUpgrade, opt => opt.MapFrom(src => src.WearIdUpgrade))
                .ForMember(dest => dest.ServerSeed, opt => opt.MapFrom(src => src.ServerSeed))
                .ForMember(dest => dest.ServerHash, opt => opt.MapFrom(src => src.ServerHash))
                .ForMember(dest => dest.UserNonce, opt => opt.MapFrom(src => src.UserNonce))
                .ForMember(dest => dest.CombinedHash, opt => opt.MapFrom(src => src.CombinedHash))
                .ForMember(dest => dest.Fraction, opt => opt.MapFrom(src => src.Fraction));
        }
    }
}
