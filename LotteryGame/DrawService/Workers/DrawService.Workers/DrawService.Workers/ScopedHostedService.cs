namespace DrawService.Workers
{
    public class ScopedHostedService<TWorker> : IHostedService 
        where TWorker : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public ScopedHostedService(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = scopeFactory.CreateScope();
            var worker = scope.ServiceProvider.GetRequiredService<TWorker>();
            
            return worker.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            var scope = scopeFactory.CreateScope();
            var worker = scope.ServiceProvider.GetRequiredService<TWorker>();
            
            return worker.StopAsync(cancellationToken);
        }
    }
}
