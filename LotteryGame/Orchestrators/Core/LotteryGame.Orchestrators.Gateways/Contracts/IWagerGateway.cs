namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using WagerService.Api.Models.Protos.History;
    using WagerService.Api.Models.Protos.Tickets;

    public interface IWagerGateway
    {
        Task<TicketProtoResponse> PurchaseTickets(int playerId, string drawId, int reservationId, int numberOfTickets);

        Task<TicketProtoResponse> UpdateTicketStatus(TicketStatusProto status, IEnumerable<string> ticketIds);

        Task<HistoryProtoResponse> GetHistory(IEnumerable<string> ticketIds);
    }
}
