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

    Console.WriteLine("Applying migrations...");

    var retryCount = 5;
    var delayMs = 5000;

    for (int i = 0; i < retryCount; i++)
    {
        try
        {
            db.Database.Migrate();
            Console.WriteLine("Database migrations applied successfully");
            break; // success, exit the loop
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration attempt {i + 1} failed: {ex.Message}");
            if (i == retryCount - 1)
                throw; // last attempt failed, rethrow
            Thread.Sleep(delayMs);
        }
    }
}

app.MapGrpcService<FundsService>();
app.MapGet("/", () => "gRPC service running on HTTP/2 localhost:5001");

app.Run();
