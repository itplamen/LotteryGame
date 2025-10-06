using WagerService.Api.Ioc;
using WagerService.Api.Services;
using WagerService.Data;
 
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);
builder.Services.AddSingleton<WagerServiceDbContext>();
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WagerServiceDbContext>();
    db.Init();
}

app.MapGrpcService<TicketService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5002");

app.Run();
