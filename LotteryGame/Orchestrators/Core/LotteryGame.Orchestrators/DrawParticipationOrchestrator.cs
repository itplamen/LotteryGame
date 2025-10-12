namespace LotteryGame.Orchestrators
{
    using AutoMapper;
    
    using DrawService.Api.Models.Protos.Draws;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawParticipation;

    public class DrawParticipationOrchestrator : IOrchestrator<DrawParticipationRequest, DrawParticipationResponse>
    {
        private readonly IMapper mapper;    
        private readonly IDrawGateway drawGateway;

        public DrawParticipationOrchestrator(IMapper mapper, IDrawGateway drawGateway)
        {
            this.mapper = mapper;
            this.drawGateway = drawGateway;
        }

        public async Task<OrchestratorResponse<DrawParticipationResponse>> Orchestrate(OrchestratorRequest<DrawParticipationRequest> request)
        {
            GetPlayerDrawProtoResponse joinedResponse = await drawGateway.JoinDraw(request.Payload.PlayerId, request.Payload.DrawId, request.Payload.TicketIds);
            return mapper.Map<OrchestratorResponse<DrawParticipationResponse>>(joinedResponse);
        }
    }
}
