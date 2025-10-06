namespace LotteryGame.Orchestrators.Gateways
{
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

        public async Task<FetchDrawResponse> GetOpenDraw(int playerId)
        {
            FetchDrawRequest fetchDrawRequest = new FetchDrawRequest()
            {
                PlayerId = playerId
            };

            return await Execute(async () => await drawClient.FetchDrawAsync(fetchDrawRequest));
        }

        public async Task<DrawResponse> JoinDraw(int playerId, string drawId, IEnumerable<string> ticketIds)
        {
            JoinDrawRequest request = new JoinDrawRequest();
            request.PlayerId = playerId;
            request.DrawId = drawId;
            request.TicketIds.AddRange(ticketIds);

            return await Execute(async () => await drawClient.JoinDrawAsync(request));
        }

        public async Task<DrawResponse> StartDraw(string drawId)
        {
            StartDrawRequest request = new StartDrawRequest()
            {
                DrawId = drawId
            };

            return await Execute(async () => await drawClient.StartDrawAsync(request));
        }
    }
}
