namespace DrawService.Workers
{
    using LotteryGame.Common.Models.Dto;
    using System.Text.Json;

    public abstract class BaseWorker : BackgroundService
    {
        private readonly int workerDelay;
        private readonly ILogger<BaseWorker> logger;

        public BaseWorker(IConfiguration configuration, ILogger<BaseWorker> logger)
        {
            this.workerDelay = int.Parse(configuration["WorkerDelay"]);
            this.logger = logger;
        }

        protected abstract Task<IEnumerable<string>> GetDrawIds();

        protected abstract Task<ResponseDto<IEnumerable<BaseDto>>> ProcessDraw(string drawId);
      
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{nameof(StartDrawWorker)} started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<string> drawIds = await GetDrawIds();

                    if (!drawIds.Any())
                    {
                        logger.LogInformation("No draws for processing");
                    }
                    else
                    {
                        logger.LogInformation($"Processing {drawIds.Count()} draws");

                        var tasks = drawIds.Select(ProcessDraw);
                        var results = await Task.WhenAll(tasks);

                        foreach (var result in results)
                        {
                            if (result.IsSuccess)
                            {
                                logger.LogInformation($"Processed draw result: {JsonSerializer.Serialize(result)}");
                            }
                            else
                            {
                                logger.LogWarning($"Failed to process: {result.ErrorMsg}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred.");
                }

                await Task.Delay(workerDelay, stoppingToken);
            }

            logger.LogInformation($"{nameof(StartDrawWorker)} stopping.");
        }
    }
}
