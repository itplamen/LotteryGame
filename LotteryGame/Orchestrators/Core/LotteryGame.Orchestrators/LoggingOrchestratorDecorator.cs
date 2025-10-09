namespace LotteryGame.Orchestrators
{
    using LotteryGame.Orchestrators.Cache.Contracts;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.Cache;

    public class LoggingOrchestratorDecorator<TRequest, TResponse> : IOrchestrator<TRequest, TResponse>
    {
        private readonly IOrchestrator<TRequest, TResponse> decoratee;
        private readonly ICacheService<OrchestratorCacheEntry<TRequest, TResponse>> cacheService;

        public LoggingOrchestratorDecorator(
            IOrchestrator<TRequest, TResponse> decoratee,
            ICacheService<OrchestratorCacheEntry<TRequest, TResponse>> cacheService)
        {
            this.decoratee = decoratee;
            this.cacheService = cacheService;
        }

        public async Task<OrchestratorResponse<TResponse>> Orchestrate(OrchestratorRequest<TRequest> request)
        {
            OrchestratorCacheEntry<TRequest, TResponse> result = await cacheService.GetOrCreateAsync(request.CorrelationId, async () =>
            {
                OrchestratorResponse<TResponse> response = await decoratee.Orchestrate(request);

                return new OrchestratorCacheEntry<TRequest, TResponse>()
                {
                    Payload = request.Payload,
                    Response = response,
                };
            });

            return result.Response;
        }
    }    
}
