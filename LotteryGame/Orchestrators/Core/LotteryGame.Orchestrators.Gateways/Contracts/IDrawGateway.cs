namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using DrawService.Api.Models.Protos.Draws;

    public interface IDrawGateway
    {
        Task<FetchDrawResponse> GetOpenDraw(int playerId);

        Task<FetchDrawResponse> CreateDraw();

        Task<FetchDrawResponse> JoinDraw(int playerId, string drawId, IEnumerable<string> ticketIds);

        Task<DrawResponse> StartDraw(string drawId);
    }
}
