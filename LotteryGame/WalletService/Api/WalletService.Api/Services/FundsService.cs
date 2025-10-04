namespace WalletService.Api.Services
{
    using System.Threading.Tasks;

    using AutoMapper;
    
    using Grpc.Core;

    using LotteryGame.Common.Models.Dto;
    using WalletService.Api.Models.Protos.Funds;
    using WalletService.Core.Contracts;

    public class FundsService : Funds.FundsBase
    {
        private readonly IMapper mapper;
        private readonly IFundsOperations fundsOperations;

        public FundsService(IMapper mapper, IFundsOperations fundsOperations)
        {
            this.mapper = mapper;
            this.fundsOperations = fundsOperations;
        }

        public override async Task<BaseResponse> HasEnoughFunds(EnoughFundsRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.HasEnoughFunds(request.PlayerId, request.CostAmount);
            BaseResponse response = mapper.Map<BaseResponse>(responseDto);

            return response;
        }

        public override async Task<ReserveResponse> Reserve(ReserveRequest request, ServerCallContext context)
        {
            ResponseDto<BaseDto> responseDto = await fundsOperations.Reserve(request.PlayerId, request.Amount);
            ReserveResponse response = mapper.Map<ReserveResponse>(responseDto);

            return response;
        }

        public override async Task<BaseResponse> Capture(CaptureRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.Capture(request.ReservationId);
            BaseResponse response = mapper.Map<BaseResponse>(responseDto);

            return response;
        }

        public override async Task<BaseResponse> Refund(RefundRequest request, ServerCallContext context)
        {
            ResponseDto responseDto = await fundsOperations.Refund(request.ReservationId);
            BaseResponse response = mapper.Map<BaseResponse>(responseDto);

            return response;
        }
    }
}
