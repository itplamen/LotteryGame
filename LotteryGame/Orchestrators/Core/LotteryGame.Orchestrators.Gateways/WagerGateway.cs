namespace LotteryGame.Orchestrators.Gateways
{
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Orchestrators.Gateways.Contracts;
    using WagerService.Api.Models.Protos.Tickets;

    public class WagerGateway : BaseGateway, IWagerGateway
    {
        private readonly Tickets.TicketsClient wagerClient;

        public WagerGateway(Tickets.TicketsClient wagerClient, IConfiguration configuration)
            : base(configuration)
        {
            this.wagerClient = wagerClient;
        }

        public async Task<TicketProtoResponse> PurchaseTickets(int playerId, string drawId, int reservationId, int numberOfTickets)
        {
            TicketCreateProtoRequest ticketCreateRequest = new TicketCreateProtoRequest()
            {
                PlayerId = playerId,
                DrawId = drawId,
                ReservationId = reservationId,
                NumberOfTickets = numberOfTickets
            };

            return await Execute(async () => await wagerClient.CreateAsync(ticketCreateRequest));
        }

        public async Task<TicketProtoResponse> UpdateTicketStatus(TicketStatusProto status, IEnumerable<string> ticketIds)
        {
            var ticketUpdateRequest = new TicketUpdateProtoRequest();
            ticketUpdateRequest.Status = status;
            ticketUpdateRequest.TicketIds.AddRange(ticketIds);

            return await Execute(async () => await wagerClient.UpdateAsync(ticketUpdateRequest));
        }
    }
}
