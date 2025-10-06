namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using WagerService.Api.Models.Protos.Tickets;

    public interface IWagerGateway
    {
        Task<TicketResponse> PurchaseTickets(int playerId, string drawId, int reservationId, int numberOfTickets);

        Task<TicketResponse> UpdateTicketStatus(TicketStatus status, IEnumerable<string> ticketIds);
    }
}
