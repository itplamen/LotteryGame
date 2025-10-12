namespace LotteryGame.Clients.Core.Services.Contracts
{
    using LotteryGame.Clients.Core.Services.Models.Betting;
    using LotteryGame.Clients.Core.Services.Models.History;
    using LotteryGame.Clients.Core.Services.Models.Profile;

    public interface ILotteryManager
    {
        Task<ProfileResponse> GetProfile(int playerId);

        Task<BettingResponse> PLayerPurchase(int playerId, int minTickets, int maxTickets);

        Task<IEnumerable<BettingResponse>> CpuPurchase(int cpuStartId, int cpuCount, int numberOfTickets);

        Task<HistoryResponse> WaitForDraw(string drawId);
    }
}
