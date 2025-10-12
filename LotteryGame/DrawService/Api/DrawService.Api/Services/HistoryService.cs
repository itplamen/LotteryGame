namespace DrawService.Api.Services
{
    using AutoMapper;

    using Grpc.Core;

    using DrawService.Api.Models.Protos.History;
    using DrawService.Core.Contracts;
    using DrawService.Core.Models;
    using LotteryGame.Common.Models.Dto;

    public class HistoryService : History.HistoryBase
    {
        private readonly IMapper mapper;
        private readonly IHistoryOperations operations;

        public HistoryService(IMapper mapper, IHistoryOperations operations)
        {
            this.mapper = mapper;
            this.operations = operations;
        }

        public override async Task<HistoryProtoResponse> Get(HistoryProtoRequest request, ServerCallContext context)
        {
            ResponseDto<HistoryDto> responseDto = await operations.GetHistory(request.DrawId);
            HistoryProtoResponse response = mapper.Map<HistoryProtoResponse>(responseDto);

            return response;
        }
    }
}
