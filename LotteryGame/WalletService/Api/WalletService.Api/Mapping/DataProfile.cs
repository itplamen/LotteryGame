namespace WalletService.Api.Mapping
{
    using AutoMapper;
    
    using WalletService.Core.Models;
    using WalletService.Data.Models;

    public class DataProfile : Profile
    {
        public DataProfile()
        {
            CreateMap<BalanceHistory, BalanceHistoryDto>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Reservation.AmountInCents))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => src.CreatedOn))
                .ForMember(dest => dest.OldBalance, opt => opt.MapFrom(src => src.OldBalanceInCents))
                .ForMember(dest => dest.NewBalance, opt => opt.MapFrom(src => src.NewBalanceInCents))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => src.Reservation.IsCaptured));
        }
    }
}
