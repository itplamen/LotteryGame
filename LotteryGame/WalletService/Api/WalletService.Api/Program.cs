using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using WalletService.Api.IoC;
using WalletService.Api.Services;
using WalletService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc(options => options.EnableDetailedErrors = true);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
long startingBalanceInCents = long.Parse(builder.Configuration["StartingBalanceInCents"]);
builder.Services.AddScoped(provider => new WalletServiceDbContext(connectionString, startingBalanceInCents));

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WalletServiceDbContext>();

    Console.WriteLine("Applying migrations");
    db.Database.Migrate();
    Console.WriteLine("Database migrations applied successfully");
}

app.MapGrpcService<FundsService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5001");

app.Run();
