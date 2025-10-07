namespace DrawService.Workers
{
    using System.Text.Json;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    using DrawService.Core.Contracts;

    public class SettlementWorker : BackgroundService
    {
        private readonly int workerDelay;
        private readonly IDrawOperations drawOperations;
        private readonly IPrizeOperations prizeOperations;
        private readonly ILogger<SettlementWorker> logger;

        public SettlementWorker(
            IDrawOperations drawOperations,
            IPrizeOperations prizeOperations,
            IConfiguration configuration,
            ILogger<SettlementWorker> logger)
        {
            this.workerDelay = int.Parse(configuration["WorkerDelay"]);
            this.drawOperations = drawOperations;
            this.prizeOperations = prizeOperations;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("SettlementWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<string> drawIds = await drawOperations.GetDrawsForSettlement();

                    if (!drawIds.Any())
                    {
                        logger.LogInformation("No draws ready for settlement.");
                    }
                    else
                    {
                        logger.LogInformation("Processing {Count} draws for settlement.", drawIds.Count());

                        var tasks = drawIds.Select(drawId => prizeOperations.DeterminePrizes(drawId));
                        var results = await Task.WhenAll(tasks);

                        foreach (var result in results)
                        {
                            if (result.IsSuccess)
                            {
                                logger.LogInformation($"Processed draw result: {JsonSerializer.Serialize(result)}");
                            }
                            else
                            {
                                logger.LogWarning($"Failed to determine prizes: {result.ErrorMsg}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred during draw settlement.");
                }

                await Task.Delay(workerDelay, stoppingToken);
            }

            logger.LogInformation("SettlementWorker stopping.");
        }
    }
}
