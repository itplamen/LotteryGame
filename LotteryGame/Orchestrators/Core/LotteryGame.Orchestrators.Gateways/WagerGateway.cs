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

        public async Task<TicketResponse> PurchaseTickets(int playerId, string drawId, int reservationId, int numberOfTickets)
        {
            TicketCreateRequest ticketCreateRequest = new TicketCreateRequest()
            {
                PlayerId = playerId,
                DrawId = drawId,
                ReservationId = reservationId,
                NumberOfTickets = numberOfTickets
            };

            return await Execute(async () => await wagerClient.CreateAsync(ticketCreateRequest));
        }

        public async Task<TicketResponse> UpdateTicketStatus(IEnumerable<string> ticketIds)
        {
            var ticketUpdateRequest = new TicketUpdateRequest();
            ticketUpdateRequest.Status = TicketStatus.Confirmed;
            ticketUpdateRequest.TicketIds.AddRange(ticketIds);

            return await Execute(async () => await wagerClient.UpdateAsync(ticketUpdateRequest));
        }
    }
}
