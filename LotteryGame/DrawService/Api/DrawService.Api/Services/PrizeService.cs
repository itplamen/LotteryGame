namespace DrawService.Api.Services
{
    using AutoMapper;

    using Grpc.Core;

    using DrawService.Api.Models.Protos.Prizes;
    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
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

        public override async Task<DeterminePrizeResponse> Determine(DeterminePrizeRequest request, ServerCallContext context)
        {
            ResponseDto<IEnumerable<PrizeDto>> responseDto = await prizeOperations.DeterminePrizes(request.DrawId);
            DeterminePrizeResponse response = mapper.Map<DeterminePrizeResponse>(responseDto);

            return response;
        }
    }
}
