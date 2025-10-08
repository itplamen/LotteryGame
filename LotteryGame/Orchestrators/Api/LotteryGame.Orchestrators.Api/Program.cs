using LotteryGame.Orchestrators.Api.IoC;
using LotteryGame.Orchestrators.Api.Services;
using LotteryGame.Common.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

app.MapGrpcService<TicketPurchaseService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5000");

app.Run();
