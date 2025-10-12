namespace LotteryGame.Orchestrators.Api.Services
{
    using AutoMapper;
    
    using Grpc.Core;
    
    using LotteryGame.Orchestrators.Api.Models.Protos.LotteryHistory;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawHistory;

    public class DrawResultService : LotteryHistory.LotteryHistoryBase
    {
        private readonly IMapper mapper;
        private IOrchestrator<DrawHistoryRequest, DrawHistoryResponse> orchestrator;

        public DrawResultService(IMapper mapper, IOrchestrator<DrawHistoryRequest, DrawHistoryResponse> orchestrator)
        {
            this.mapper = mapper;
            this.orchestrator = orchestrator;
        }

        public async override Task<HistoryProtoResponse> Get(HistoryProtoRequest request, ServerCallContext context)
        {
            OrchestratorRequest<DrawHistoryRequest> drawHistory = new(new DrawHistoryRequest { DrawId = request.DrawId });
            OrchestratorResponse<DrawHistoryResponse> drawHistoryResponse = await orchestrator.Orchestrate(drawHistory);

            return mapper.Map<HistoryProtoResponse>(drawHistoryResponse);
        }
    }
}
