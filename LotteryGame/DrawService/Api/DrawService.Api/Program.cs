using System.Text.Json.Serialization;

using DrawService.Api.IoC;
using DrawService.Api.Services;
using LotteryGame.Common.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});
builder.Services.AddServices(builder.Configuration);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

var app = builder.Build();

app.MapGrpcService<DrawsService>();
app.MapGrpcService<PrizeService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5003");

app.Run();
