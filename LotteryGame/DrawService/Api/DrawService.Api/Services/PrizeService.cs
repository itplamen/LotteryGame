namespace DrawService.Api.Services
{
    using AutoMapper;

    using Grpc.Core;

    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using DrawService.Api.Models.Protos.Prizes;
    using LotteryGame.Common.Models.Dto;

    public class PrizeService : Prizes.PrizesBase
    {
        private readonly IMapper mapper;
        private readonly IPrizeOperations prizeOperations;

        public PrizeService(IMapper mapper, IPrizeOperations prizeOperations)
        {
            this.mapper = mapper;
            this.prizeOperations = prizeOperations;
        }

        public override async Task<DeterminePrizeProtoResponse> Determine(DeterminePrizeProtoRequest request, ServerCallContext context)
        {
            ResponseDto<IEnumerable<PrizeDto>> responseDto = await prizeOperations.DeterminePrizes(request.DrawId);
            DeterminePrizeProtoResponse response = mapper.Map<DeterminePrizeProtoResponse>(responseDto);

            return response;
        }
    }
}
