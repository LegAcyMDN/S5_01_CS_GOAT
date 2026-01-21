using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class FairRandomMapper : Profile
    {
        public FairRandomMapper()
        {
            _ = CreateMap<FairRandom, FairRandomDTO>()
                .ForMember(dest => dest.ServerSeed, opt => opt.MapFrom(src => src.ServerSeed))
                .ForMember(dest => dest.ServerHash, opt => opt.MapFrom(src => src.ServerHash))
                .ForMember(dest => dest.UserSeed, opt => opt.MapFrom(src => src.UserSeed))
                .ForMember(dest => dest.UserNonce, opt => opt.MapFrom(src => src.UserNonce))
                .ForMember(dest => dest.CombinedHash, opt => opt.MapFrom(src => src.CombinedHash))
                .ForMember(dest => dest.Fraction1, opt => opt.MapFrom(src => src.Fraction1))
                .ForMember(dest => dest.Fraction2, opt => opt.MapFrom(src => src.Fraction2))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src =>
                    src.GetRandomTransaction() != null ? src.GetRandomTransaction()!.TransactionId : (int?)null));
        }
    }
}
