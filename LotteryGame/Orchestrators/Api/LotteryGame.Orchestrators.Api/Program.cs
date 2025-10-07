using DrawService.Api.Models.Protos.Draws;
using LotteryGame.Orchestrators.Api.Services;
using WagerService.Api.Models.Protos.Tickets;
using WalletService.Api.Models.Protos.Funds;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpcClient<Funds.FundsClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Grpc:WalletServiceUrl"]);
});

builder.Services.AddGrpcClient<Tickets.TicketsClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Grpc:WagerServiceUrl"]);
});

builder.Services.AddGrpcClient<Draws.DrawsClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Grpc:DrawServiceUrl"]);
});

// Add orchestrator service
builder.Services.AddScoped<TicketPurchaseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TicketPurchaseService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");


app.Run();
