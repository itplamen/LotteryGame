namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using DrawService.Api.Models.Protos.Draws;

    public interface IDrawGateway
    {
        Task<DrawOptionsProtoResponse> GetDrawOptions();

        Task<FetchDrawProtoResponse> GetOpenDraw(int playerId);

        Task<FetchDrawProtoResponse> CreateDraw();

        Task<FetchDrawProtoResponse> JoinDraw(int playerId, string drawId, IEnumerable<string> ticketIds);
    }
}
