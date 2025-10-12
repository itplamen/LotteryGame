namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using DrawService.Api.Models.Protos.Draws;

    public interface IDrawGateway
    {
        Task<DrawOptionsProtoResponse> GetDrawOptions();

        Task<GetPlayerDrawProtoResponse> GetOpenDraw(int playerId);

        Task<GetPlayerDrawProtoResponse> CreateDraw();

        Task<GetPlayerDrawProtoResponse> JoinDraw(int playerId, string drawId, IEnumerable<string> ticketIds);
    }
}
