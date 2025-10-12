namespace LotteryGame.Clients.Core.Services.Contracts
{
    using LotteryGame.Clients.Core.Services.Models;

    public interface ILotteryService
    {
        Task<PlayerProfileResponse> GetProfile(int playerId);

        Task<PurchaseTicketsResponse> PurchaseTickets(int playerId, int numberOfTickets);
    }
}
