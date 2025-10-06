namespace LotteryGame.Orchestrators.Gateways
{
    using Microsoft.Extensions.Configuration;

    public abstract class BaseGateway
    {
        private readonly int maxRetries;
        private readonly int retryDelayMs;

        public BaseGateway(IConfiguration configuration)
        {
            maxRetries = int.Parse(configuration["Retry:Max"]);
            retryDelayMs = int.Parse(configuration["Retry:DelayMs"]);
        }

        public async Task<T> Execute<T>(Func<Task<T>> action)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    return await action();
                }
                catch
                {
                    if (i == maxRetries - 1) throw;
                    await Task.Delay(retryDelayMs * (i + 1));
                }
            }

            throw new InvalidOperationException("Retry failed");
        }
    }
}
