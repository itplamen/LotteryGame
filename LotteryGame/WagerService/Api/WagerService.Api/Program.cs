using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
 
using Microsoft.Extensions.DependencyInjection;
using WagerService.Api.Ioc;
using WagerService.Api.Services;
using WagerService.Data;
 
var builder = WebApplication.CreateBuilder(args);

// gRPC with detailed errors
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);

// EF DbContext with retry policy
builder.Services.AddSingleton<WagerServiceDbContext>();

// Register custom services (IoC)
builder.Services.AddServices(builder.Configuration);

// Kestrel: HTTP/2 without TLS (must use localhost!)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5002, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WagerServiceDbContext>();
    db.Init();
}



// Map gRPC service
app.MapGrpcService<TicketService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5001");

app.Run();
