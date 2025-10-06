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
    }
}
