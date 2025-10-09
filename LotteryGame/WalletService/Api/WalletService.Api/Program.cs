using Microsoft.EntityFrameworkCore;

using LotteryGame.Common.Utils;
using WalletService.Api.IoC;
using WalletService.Api.Services;
using WalletService.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.Interceptors.Add<GrpcExceptionInterceptor>();
});

builder.Services.AddDbContext<WalletServiceDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    ));

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    Console.WriteLine("Applying migrations... ");
    
    var db = scope.ServiceProvider.GetRequiredService<WalletServiceDbContext>();
    db.Database.Migrate();

    Console.WriteLine("Migrations successfully applied!");
}

app.MapGrpcService<FundsService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5001");

app.Run();
