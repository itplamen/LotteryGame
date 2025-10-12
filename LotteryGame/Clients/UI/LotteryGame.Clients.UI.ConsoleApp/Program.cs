using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using LotteryGame.Clients.Core.Services.Contracts;
using LotteryGame.Clients.Core.Services.Models.Betting;
using LotteryGame.Clients.Core.Services.Models.History;
using LotteryGame.Clients.Core.Services.Models.Profile;
using LotteryGame.Clients.Core.Services.Services;
using LotteryGame.Clients.Core.Wrapper.Contracts;
using LotteryGame.Clients.UI.ConsoleApp;
using LotteryGame.Orchestrators.Api.Models.Protos.LotteryHistory;
using LotteryGame.Orchestrators.Api.Models.Protos.PlayerProfile;
using LotteryGame.Orchestrators.Api.Models.Protos.TicketPurchase;
using LotteryGame.Clients.Core.Services.Managers;

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

        services.AddSingleton<ILotteryService<ProfileRequest, ProfileResponse>, LotteryProfileService>();
        services.AddSingleton<ILotteryService<BettingRequest, BettingResponse>, LotteryBettingService>();
        services.AddSingleton<ILotteryService<HistoryRequest, HistoryResponse>, LotteryHistoryService>();
        services.AddSingleton<IClientManager, ConsoleClientManager>();
        services.AddSingleton<IProgramExecutor, ConsoleProgramExecutor>();
        services.AddSingleton<LotteryManager>();
        services.AddSingleton<ILotteryManager>(sp =>
        {
            var clientManager = sp.GetRequiredService<IClientManager>();
            var baseManager = sp.GetRequiredService<LotteryManager>();
            return new LoggingLotteryManagerDecorator(baseManager, clientManager);
        });
    })
    .Build();

var programExecutor = host.Services.GetRequiredService<IProgramExecutor>();
await programExecutor.Run();