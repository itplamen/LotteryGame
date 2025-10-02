using LotteryGame.Orchestrators.Services;
using WagerService.Api.Models.Protos.Tickets;
using WalletService.Api.Models.Protos.Funds;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TicketPurchaseOrchestrator>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

builder.Services.AddGrpcClient<Funds.FundsClient>(x =>
{
    x.Address = new Uri("https://wagerservice:5001");
});
builder.Services.AddGrpcClient<Tickets.TicketsClient>(x =>
{
    x.Address = new Uri("https://wagerservice:5002");
});

app.Run();
