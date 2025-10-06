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

        public override async Task<BaseResponse> HasEnoughFunds(EnoughFundsRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.HasEnoughFunds(request.PlayerId, request.Cost);
            BaseResponse response = mapper.Map<BaseResponse>(responseDto);

            return response;
        }

        public override async Task<ReserveResponse> Reserve(ReserveRequest request, ServerCallContext context)
        {
            ResponseDto<BaseDto> responseDto = await fundsOperations.Reserve(request.PlayerId, request.Amount);
            ReserveResponse response = mapper.Map<ReserveResponse>(responseDto);

            return response;
        }

        public override async Task<BaseResponse> Capture(FundsRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.Capture(request.ReservationId);
            BaseResponse response = mapper.Map<BaseResponse>(responseDto);

            return response;
        }

        public override async Task<BaseResponse> Refund(FundsRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.Refund(request.ReservationId);
            BaseResponse response = mapper.Map<BaseResponse>(responseDto);

            return response;
        }

        public override async Task<HistoryResponseList> GetHistory(HistoryRequest request, ServerCallContext context)
        {
            IEnumerable<BalanceHistoryDto> historyDtos = await balanceHistoryOperations.Get(request.PlayerId);
            HistoryResponseList response = mapper.Map<HistoryResponseList>(historyDtos);

            return response;
        }
    }
}
