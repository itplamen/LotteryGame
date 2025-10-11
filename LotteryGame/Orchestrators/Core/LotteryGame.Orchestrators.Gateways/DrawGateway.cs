namespace LotteryGame.Orchestrators.Gateways
{
    using Google.Protobuf.WellKnownTypes;

    using Microsoft.Extensions.Configuration;

    using DrawService.Api.Models.Protos.Draws;
    using LotteryGame.Orchestrators.Gateways.Contracts;

    public class DrawGateway : BaseGateway, IDrawGateway
    {
        private readonly Draws.DrawsClient drawClient;

        public DrawGateway(Draws.DrawsClient drawClient, IConfiguration configuration)
            : base(configuration)
        {
            this.drawClient = drawClient;
        }

        public async Task<DrawOptionsProtoResponse> GetDrawOptions() => 
            await Execute(async () => await drawClient.GetDrawOptionsAsync(new Empty()));

        public async Task<FetchDrawProtoResponse> GetOpenDraw(int playerId)
        {
            FetchDrawProtoRequest fetchDrawRequest = new FetchDrawProtoRequest()
            {
                PlayerId = playerId
            };

            return await Execute(async () => await drawClient.FetchDrawAsync(fetchDrawRequest));
        }

        public async Task<FetchDrawProtoResponse> CreateDraw() => await Execute(async () => await drawClient.CreateDrawAsync(new Empty()));

        public async Task<FetchDrawProtoResponse> JoinDraw(int playerId, string drawId, IEnumerable<string> ticketIds)
        {
            JoinDrawProtoRequest request = new JoinDrawProtoRequest();
            request.PlayerId = playerId;
            request.DrawId = drawId;
            request.TicketIds.AddRange(ticketIds);

            return await Execute(async () => await drawClient.JoinDrawAsync(request));
        }
    }
}
