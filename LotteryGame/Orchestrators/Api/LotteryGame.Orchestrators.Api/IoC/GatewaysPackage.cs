namespace LotteryGame.Orchestrators.Api.IoC
{
    using DrawService.Api.Models.Protos.Draws;
    using DrawService.Api.Models.Protos.History;
    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Orchestrators.Gateways;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using WagerService.Api.Models.Protos.History;
    using WagerService.Api.Models.Protos.Tickets;
    using WalletService.Api.Models.Protos.Funds;

    public sealed class GatewaysPackage : IPackage
    {
        private readonly IConfiguration configuration;

        public GatewaysPackage(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<IDrawGateway, DrawGateway>();
            services.AddSingleton<IWagerGateway, WagerGateway>();
            services.AddSingleton<IWalletGateway, WalletGateway>();
            
            services.AddGrpcClient<Draws.DrawsClient>(x =>
            {
                x.Address = new Uri(configuration["Grpc:DrawServiceUrl"]);
            });

            services.AddGrpcClient<DrawHistory.DrawHistoryClient>(x =>
            {
                x.Address = new Uri(configuration["Grpc:DrawServiceUrl"]);
            });

            services.AddGrpcClient<Tickets.TicketsClient>(x =>
            {
                x.Address = new Uri(configuration["Grpc:WagerServiceUrl"]);
            });

            services.AddGrpcClient<WagerHistory.WagerHistoryClient>(x =>
            {
                x.Address = new Uri(configuration["Grpc:WagerServiceUrl"]);
            });

            services.AddGrpcClient<Funds.FundsClient>(x =>
            {
                x.Address = new Uri(configuration["Grpc:WalletServiceUrl"]);
            });
        }
    }
}
