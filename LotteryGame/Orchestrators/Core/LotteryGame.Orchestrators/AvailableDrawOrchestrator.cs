namespace LotteryGame.Orchestrators
{
    using AutoMapper;
    
    using DrawService.Api.Models.Protos.Draws;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.Base;

    public class AvailableDrawOrchestrator : IOrchestrator<AvailableDrawRequest, AvailableDrawResponse>
    {
        private readonly IMapper mapper;
        private readonly IDrawGateway gateway;

        public AvailableDrawOrchestrator(IMapper mapper, IDrawGateway gateway)
        {
            this.mapper = mapper;
            this.gateway = gateway;
        }

        public async Task<OrchestratorResponse<AvailableDrawResponse>> Orchestrate(OrchestratorRequest<AvailableDrawRequest> request)
        {
            FetchDrawResponse fetchDrawResponse = await gateway.GetOpenDraw(request.Payload.PlayerId);

            if (!fetchDrawResponse.Success)
            {
                fetchDrawResponse = await gateway.CreateDraw();
            }
            
            return mapper.Map<OrchestratorResponse<AvailableDrawResponse>>(fetchDrawResponse);
        }
    }
}
