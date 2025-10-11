namespace WalletService.Api.Mapping
{
    using AutoMapper;
   
    using Google.Protobuf.WellKnownTypes;
    
    using LotteryGame.Common.Models.Dto;
    using WalletService.Api.Models.Protos.Funds;
    using WalletService.Core.Models;
    using WalletService.Data.Models;

    public class ProtosProfile : Profile
    {
        public ProtosProfile()
        {
            CreateMap<ResponseDto, BaseProtoResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg));

            CreateMap<ResponseDto<BaseDto>, ReserveProtoResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.ReservationId, opt => opt.MapFrom(src => src.Data.Id));

            CreateMap<ResponseDto<WalletDto>, WalletProtoResponse>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.IsSuccess))
                .ForMember(dest => dest.ErrorMsg, opt => opt.MapFrom(src => src.ErrorMsg))
                .ForMember(dest => dest.RealMoneyInCents, opt => opt.MapFrom(src => src.Data.RealMoneyInCents))
                 .ForMember(dest => dest.BonusMoneyInCents, opt => opt.MapFrom(src => src.Data.BonusMoneyInCents));

            CreateMap<BalanceHistoryDto, HistoryProtoResponse>()
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.CreatedOn.ToUniversalTime())))
                .ForMember(dest => dest.OldBalance, opt => opt.MapFrom(src => src.OldBalance))
                .ForMember(dest => dest.NewBalance, opt => opt.MapFrom(src => src.NewBalance))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => MapBalanceType(src.Type)))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.IsConfirmed, opt => opt.MapFrom(src => src.IsConfirmed));

            CreateMap<IEnumerable<BalanceHistoryDto>, HistoryProtoResponseList>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src));
        }

        private static BalanceTypeProto MapBalanceType(BalanceType type)
        {
            return type switch
            {
                BalanceType.Reserve => BalanceTypeProto.Reserve,
                BalanceType.Capture => BalanceTypeProto.Capture,
                BalanceType.Refund => BalanceTypeProto.Refund,
                BalanceType.RealMoneyPrize => BalanceTypeProto.RealMoneyPrize,
                BalanceType.BonusMoneyPrize => BalanceTypeProto.BonusMoneyPrize,
                BalanceType.LoyaltyPointsPrize => BalanceTypeProto.LoyaltyPointsPrize,
                _ => throw new ArgumentOutOfRangeException("Invalid balance type")
            };
        }
    }
}
