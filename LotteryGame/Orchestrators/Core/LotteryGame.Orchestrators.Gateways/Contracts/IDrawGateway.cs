namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using DrawService.Api.Models.Protos.Draws;

    public interface IDrawGateway
    {
        Task<FetchDrawResponse> GetOpenDraw(int playerId);
    }
}
