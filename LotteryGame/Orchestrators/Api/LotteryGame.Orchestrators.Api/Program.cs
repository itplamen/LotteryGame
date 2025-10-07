using LotteryGame.Orchestrators.Api.IoC;
using LotteryGame.Orchestrators.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<TicketPurchaseService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5000");

app.Run();
