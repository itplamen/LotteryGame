using AutoMapper;
using DrawService.Api.IoC;
using DrawService.Api.Services;
using DrawService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json.Serialization;


 
var builder = WebApplication.CreateBuilder(args);

// gRPC with detailed errors
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);

 



// Register custom services (IoC)
builder.Services.AddServices(builder.Configuration);

// Kestrel: HTTP/2 without TLS (must use localhost!)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5003, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
           options.JsonSerializerOptions.WriteIndented = true; // optional, for pretty JSON
       });

var app = builder.Build();

 


// Map gRPC service
app.MapGrpcService<DrawsService>();
app.MapGrpcService<PrizeService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5001");

app.Run();
