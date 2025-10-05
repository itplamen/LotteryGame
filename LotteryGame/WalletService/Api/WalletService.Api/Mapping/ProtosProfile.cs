namespace WalletService.Api.Mapping
{
    using AutoMapper;
   
    using Google.Protobuf.WellKnownTypes;
    
    using LotteryGame.Common.Models.Dto;
    using WalletService.Api.Models.Protos.Funds;
    using WalletService.Core.Models;

    public class ProtosProfile : Profile
    {
        public ProtosProfile()
        {
            CreateMap<ResponseDto, BaseResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

            CreateMap<ResponseDto<BaseDto>, ReserveResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Data.Id));

            CreateMap<BalanceHistoryDto, HistoryResponse>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedOn.ToUniversalTime())))
                .ForMember(dest => dest.OldBalance, opt => opt.MapFrom(src => src.OldBalance))
                .ForMember(dest => dest.NewBalance, opt => opt.MapFrom(src => src.NewBalance))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MapBalanceType(src.Type)))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => src.IsConfirmed));

            CreateMap<IEnumerable<BalanceHistoryDto>, HistoryResponseList>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src));
        }

        private static BalanceType MapBalanceType(Data.Models.BalanceType type)
        {
            return type switch
            {
                Data.Models.BalanceType.Reserve => BalanceType.Reserve,
                Data.Models.BalanceType.Capture => BalanceType.Capture,
                Data.Models.BalanceType.Refund => BalanceType.Refund,
                Data.Models.BalanceType.RealMoneyPrize => BalanceType.RealMoneyPrize,
                Data.Models.BalanceType.BonusMoneyPrize => BalanceType.BonusMoneyPrize,
                Data.Models.BalanceType.LoyaltyPointsPrize => BalanceType.LoyaltyPointsPrize,
                _ => throw new ArgumentOutOfRangeException("Invalid balance type")
            };
        }
    }
}
