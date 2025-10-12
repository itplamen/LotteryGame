namespace WagerService.Api.Services
{
    using AutoMapper;
    
    using Grpc.Core;
    
    using LotteryGame.Common.Models.Dto;
    using WagerService.Api.Models.Protos.History;
    using WagerService.Core.Contracts;
    using WagerService.Core.Models;

    public class HistoryService : WagerHistory.WagerHistoryBase
    {
        private readonly IMapper mapper;
        private readonly IHistoryOperations historyOperations;

        public HistoryService(IMapper mapper, IHistoryOperations historyOperations)
        {
            this.mapper = mapper;
            this.historyOperations = historyOperations;
        }

        public override async Task<HistoryProtoResponse> Get(HistoryProtoRequest request, ServerCallContext context)
        {
            ResponseDto<IEnumerable<TicketDto>> responseDto = await historyOperations.GetHistory(request.TicketIds);
            HistoryProtoResponse response = mapper.Map<HistoryProtoResponse>(responseDto);

            return response;
        }
    }
}
