namespace LotteryGame.Orchestrators
{
    using AutoMapper;
    
    using DrawHistory = DrawService.Api.Models.Protos.History;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawHistory;
    using WagerService.Api.Models.Protos.History;

    public class DrawHistoryOrchestrator : IOrchestrator<DrawHistoryRequest, DrawHistoryResponse>
    {
        private readonly IMapper mapper;
        private readonly IDrawGateway drawGateway;
        private readonly IWagerGateway wagerGateway;

        public DrawHistoryOrchestrator(IMapper mapper, IDrawGateway drawGateway, IWagerGateway wagerGateway)
        {
            this.mapper = mapper;
            this.drawGateway = drawGateway;
            this.wagerGateway = wagerGateway;
        }

        public async Task<OrchestratorResponse<DrawHistoryResponse>> Orchestrate(OrchestratorRequest<DrawHistoryRequest> request)
        {
            DrawHistory.HistoryProtoResponse protoResponse = await drawGateway.GetHistory(request.Payload.DrawId);

            if (!protoResponse.Success)
            {
                return mapper.Map<OrchestratorResponse<DrawHistoryResponse>>(protoResponse);
            }

            IEnumerable<string> ticketIds = protoResponse.Draw.Prizes.Select(x => x.TicketId).ToList();
            HistoryProtoResponse wagerResponse = await wagerGateway.GetHistory(ticketIds);

            if (!wagerResponse.Success)
            {
                return mapper.Map<OrchestratorResponse<DrawHistoryResponse>>(wagerResponse);
            }

            OrchestratorResponse<DrawHistoryResponse> response = mapper.Map<OrchestratorResponse<DrawHistoryResponse>>(protoResponse);
            response.Data.Prizes = protoResponse.Draw.Prizes.Select(x =>
            {
                TicketHistoryProto ticket = wagerResponse.Tickets.FirstOrDefault(y => y.Id == x.TicketId);

                return new PrizeHistory()
                {
                    Tier = x.Tier,
                    AmountInCents = x.AmountInCents,
                    TicketNumber = ticket?.TicketNumber ?? "",
                    Status = ticket?.Status.ToString() ?? ""
                };
            });

            return response;
        }
    }
}
