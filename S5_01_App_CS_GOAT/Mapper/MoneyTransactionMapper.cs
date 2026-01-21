using AutoMapper;
using S5_01_App_CS_GOAT.Models.EntityFramework;
using Shared.DTO;

namespace S5_01_App_CS_GOAT.Mapper
{
    public class MoneyTransactionMapper : Profile
    {
        public MoneyTransactionMapper()
        {
            // Entity -> DetailDTO
            _ = CreateMap<MoneyTransaction, MoneyTransactionDTO>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.PaymentMethodName))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.WalletValue, opt => opt.MapFrom(src => src.WalletValue))
                .ForMember(dest => dest.CancelledOn, opt => opt.MapFrom(src => src.CancelledOn));
        }
    }
}
