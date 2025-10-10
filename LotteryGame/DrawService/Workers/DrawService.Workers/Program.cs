using DrawService.Workers;
using DrawService.Workers.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddServices(builder.Configuration);

builder.Services.AddScoped<StartDrawWorker>();
builder.Services.AddScoped<SettlementWorker>();
builder.Services.AddSingleton<IHostedService>(x =>
{
    return new ScopedHostedService<StartDrawWorker>(
        x.GetRequiredService<IServiceScopeFactory>()
    );
});
builder.Services.AddSingleton<IHostedService>(x =>
{
    return new ScopedHostedService<SettlementWorker>(
        x.GetRequiredService<IServiceScopeFactory>()
    );
});

var host = builder.Build();
host.Run();
