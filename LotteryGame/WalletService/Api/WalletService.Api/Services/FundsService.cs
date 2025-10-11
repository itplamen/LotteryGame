namespace WalletService.Api.Services
{
    using System.Threading.Tasks;

    using AutoMapper;
    
    using Grpc.Core;
    
    using LotteryGame.Common.Models.Dto;
    using WalletService.Api.Models.Protos.Funds;
    using WalletService.Core.Contracts;
    using WalletService.Core.Models;

    public class FundsService : Funds.FundsBase
    {
        private readonly IMapper mapper;
        private readonly IFundsOperations fundsOperations;
        private readonly IBalanceHistoryOperations balanceHistoryOperations;

        public FundsService(IMapper mapper, IFundsOperations fundsOperations, IBalanceHistoryOperations balanceHistoryOperations)
        {
            this.mapper = mapper;
            this.fundsOperations = fundsOperations;
            this.balanceHistoryOperations = balanceHistoryOperations;
        }

        public override async Task<BaseProtoResponse> HasEnoughFunds(EnoughFundsProtoRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.HasEnoughFunds(request.PlayerId, request.Cost);
            BaseProtoResponse response = mapper.Map<BaseProtoResponse>(responseDto);

            return response;
        }

        public override async Task<WalletProtoResponse> GetFunds(WalletProtoRequest request, ServerCallContext context)
        {
            ResponseDto<WalletDto> responseDto = await fundsOperations.GetFunds(request.PlayerId);
            WalletProtoResponse response = mapper.Map<WalletProtoResponse>(responseDto);

            return response;
        }

        public override async Task<ReserveProtoResponse> Reserve(ReserveProtoRequest request, ServerCallContext context)
        {
            ResponseDto<BaseDto> responseDto = await fundsOperations.Reserve(request.PlayerId, request.Amount);
            ReserveProtoResponse response = mapper.Map<ReserveProtoResponse>(responseDto);

            return response;
        }

        public override async Task<BaseProtoResponse> Capture(FundsProtoRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.Capture(request.ReservationId);
            BaseProtoResponse response = mapper.Map<BaseProtoResponse>(responseDto);

            return response;
        }

        public override async Task<BaseProtoResponse> Refund(FundsProtoRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.Refund(request.ReservationId);
            BaseProtoResponse response = mapper.Map<BaseProtoResponse>(responseDto);

            return response;
        }

        public override async Task<HistoryProtoResponseList> GetHistory(HistoryProtoRequest request, ServerCallContext context)
        {
            IEnumerable<BalanceHistoryDto> historyDtos = await balanceHistoryOperations.Get(request.PlayerId);
            HistoryProtoResponseList response = mapper.Map<HistoryProtoResponseList>(historyDtos);

            return response;
        }
    }
}
