using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using LotteryGame.Clients.Core.Services.Contracts;
using LotteryGame.Clients.Core.Services.Services;
using LotteryGame.Clients.Core.Wrapper.Contracts;
using LotteryGame.Clients.UI.ConsoleApp;
using LotteryGame.Orchestrators.Api.Models.Protos.PlayerProfile;
using LotteryGame.Orchestrators.Api.Models.Protos.TicketPurchase;
using LotteryGame.Orchestrators.Api.Models.Protos.LotteryHistory;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddGrpcClient<PlayerProfile.PlayerProfileClient>(options =>
        {
            options.Address = new Uri(context.Configuration["Grpc:OrchestratorUrl"]);
        });

        services.AddGrpcClient<TicketPurchase.TicketPurchaseClient>(options =>
        {
            options.Address = new Uri(context.Configuration["Grpc:OrchestratorUrl"]); 
        });

        services.AddGrpcClient<LotteryHistory.LotteryHistoryClient>(options =>
        {
            options.Address = new Uri(context.Configuration["Grpc:OrchestratorUrl"]);
        });

        services.AddSingleton<ILotteryService, LotteryService>();
        services.AddSingleton<IClientManager, ConsoleClientManager>();
        services.AddSingleton<IProgramManager, ProgramManager>();
    })
    .Build();

var programManager = host.Services.GetRequiredService<IProgramManager>();
await programManager.Run();