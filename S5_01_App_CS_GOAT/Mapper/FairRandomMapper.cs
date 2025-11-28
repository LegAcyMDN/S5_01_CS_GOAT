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
                .ForMember(dest => dest.ServerSeed, opt => opt.MapFrom(src => src.ServerSeed))
                .ForMember(dest => dest.ServerHash, opt => opt.MapFrom(src => src.ServerHash))
                .ForMember(dest => dest.UserNonce, opt => opt.MapFrom(src => src.UserNonce))
                .ForMember(dest => dest.CombinedHash, opt => opt.MapFrom(src => src.CombinedHash))
                .ForMember(dest => dest.Fraction, opt => opt.MapFrom(src => src.Fraction))
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.RandomTransaction != null ? src.RandomTransaction.TransactionId : (int?)null))
                .ForMember(dest => dest.ItemWearId, opt => opt.MapFrom(src => src.UpgradeResult != null ? src.UpgradeResult.InventoryItem.WearId : (int?)null))
                .ForMember(dest => dest.ItemUserId, opt => opt.MapFrom(src => src.UpgradeResult != null ? src.UpgradeResult.InventoryItem.UserId : (int?)null));
        }
    }
}
