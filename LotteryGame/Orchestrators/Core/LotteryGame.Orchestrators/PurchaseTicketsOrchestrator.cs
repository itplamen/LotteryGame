namespace LotteryGame.Orchestrators
{
    using AutoMapper;

    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using WagerService.Api.Models.Protos.Tickets;
    using WalletService.Api.Models.Protos.Funds;

    public class PurchaseTicketsOrchestrator : IOrchestrator<PurchaseTicketsRequest, PurchaseTicketsResponse>
    {
        private readonly IMapper mapper;
        private readonly IWagerGateway wagerGateway;
        private readonly IWalletGateway walletGateway;

        public PurchaseTicketsOrchestrator(IMapper mapper, IWagerGateway wagerGateway, IWalletGateway walletGateway)
        {
            this.mapper = mapper;
            this.wagerGateway = wagerGateway;
            this.walletGateway = walletGateway;
        }

        public async Task<OrchestratorResponse<PurchaseTicketsResponse>> Orchestrate(OrchestratorRequest<PurchaseTicketsRequest> request)
        {
            TicketProtoResponse ticketResponse = await wagerGateway.PurchaseTickets(request.Payload.PlayerId, request.Payload.DrawId, request.Payload.ReservationId, request.Payload.NumberOfTickets);
            
            if (!ticketResponse.Success)
            {
                BaseProtoResponse baseResponse = await walletGateway.RefundFunds(request.Payload.ReservationId);
                return mapper.Map<OrchestratorResponse<PurchaseTicketsResponse>>(baseResponse);
            }

            IEnumerable<string> ticketIds = ticketResponse.Tickets.Select(x => x.Id).ToList();
            BaseProtoResponse captureResponse = await walletGateway.CaptureFunds(request.Payload.ReservationId);

            if (!captureResponse.Success)
            {
                TicketProtoResponse cancelledResponse = await wagerGateway.UpdateTicketStatus(TicketStatusProto.Cancelled, ticketIds);
                return mapper.Map<OrchestratorResponse<PurchaseTicketsResponse>>(cancelledResponse);
            }

            TicketProtoResponse confirmedResponse = await wagerGateway.UpdateTicketStatus(TicketStatusProto.Confirmed, ticketIds);
            return mapper.Map<OrchestratorResponse<PurchaseTicketsResponse>>(confirmedResponse);
        }
    }
}
