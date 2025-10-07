namespace LotteryGame.Orchestrators.Api.Services
{
    using AutoMapper;
    
    using Grpc.Core;
    
    using LotteryGame.Orchestrators.Api.Models.Protos.TicketPurchase;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawParticipation;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using LotteryGame.Orchestrators.Models.ReserveFunds;

    public class TicketPurchaseService : TicketPurchase.TicketPurchaseBase
    {
        private readonly IMapper mapper;
        private readonly IOrchestrator<AvailableDrawRequest, AvailableDrawResponse> availableDraw;
        private readonly IOrchestrator<ReserveFundsRequest, ReserveFundsResponse> reserveFunds; 
        private readonly IOrchestrator<PurchaseTicketsRequest, PurchaseTicketsResponse> purchaseTickets;
        private readonly IOrchestrator<DrawParticipationRequest, DrawParticipationResponse> drawParticipation;

        public TicketPurchaseService(
            IMapper mapper,
            IOrchestrator<AvailableDrawRequest, AvailableDrawResponse> availableDraw, 
            IOrchestrator<ReserveFundsRequest, ReserveFundsResponse> reserveFunds, 
            IOrchestrator<PurchaseTicketsRequest, PurchaseTicketsResponse> purchaseTickets, 
            IOrchestrator<DrawParticipationRequest, DrawParticipationResponse> drawParticipation)
        {
            this.mapper = mapper;
            this.availableDraw = availableDraw;
            this.reserveFunds = reserveFunds;
            this.purchaseTickets = purchaseTickets;
            this.drawParticipation = drawParticipation;
        }

        public async override Task<PurchaseResponse> Purchase(PurchaseRequest request, ServerCallContext context)
        {
            OrchestratorRequest<AvailableDrawRequest> availableDrawRequest = new (new AvailableDrawRequest { PlayerId = request.PlayerId });
            OrchestratorResponse<AvailableDrawResponse> availableDrawResponse = await availableDraw.Orchestrate(availableDrawRequest);

            if (availableDrawResponse.Success)
            {
                OrchestratorRequest<ReserveFundsRequest> reserveFundsRequest = mapper.Map<OrchestratorRequest<ReserveFundsRequest>>(request);
                reserveFundsRequest.Payload.TicketPriceInCents = availableDrawResponse.Data.TicketPriceInCents;

                OrchestratorResponse<ReserveFundsResponse> reserveFundsResponse = await reserveFunds.Orchestrate(reserveFundsRequest);

                if (reserveFundsResponse.Success)
                {
                    OrchestratorRequest<PurchaseTicketsRequest> purchaseTicketsRequest = mapper.Map<OrchestratorRequest<PurchaseTicketsRequest>>(request);
                    purchaseTicketsRequest.Payload.ReservationId = reserveFundsResponse.Data.ReservationId;
                    purchaseTicketsRequest.Payload.DrawId = availableDrawResponse.Data.DrawId;

                    OrchestratorResponse<PurchaseTicketsResponse> purchaseTicketsResponse = await purchaseTickets.Orchestrate(purchaseTicketsRequest);

                    if (purchaseTicketsResponse.Success)
                    {
                        OrchestratorRequest<DrawParticipationRequest> drawParticipationRequest = mapper.Map<OrchestratorRequest<DrawParticipationRequest>>(purchaseTicketsRequest);
                        drawParticipationRequest.Payload.TicketIds = purchaseTicketsResponse.Data.Tickets.Select(x => x.Id).ToList();

                        await drawParticipation.Orchestrate(drawParticipationRequest);
                    }
                }
            }

            return new PurchaseResponse()
            {
                Success = false,
                ErrorMsg = "Could not purchase tickets"
            };
        }
    }
}