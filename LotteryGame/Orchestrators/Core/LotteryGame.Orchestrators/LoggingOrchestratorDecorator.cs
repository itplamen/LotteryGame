namespace LotteryGame.Orchestrators
{
    using LotteryGame.Orchestrators.Cache.Contracts;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.Base;

    public class LoggingOrchestratorDecorator<TRequest, TResponse> : IOrchestrator<TRequest, TResponse>
    {
        private readonly IOrchestrator<TRequest, TResponse> decoratee;
        private readonly ICacheService<IDictionary<TRequest, OrchestratorResponse<TResponse>>> cacheService;

        public LoggingOrchestratorDecorator(
            IOrchestrator<TRequest, TResponse> decoratee, 
            ICacheService<IDictionary<TRequest, OrchestratorResponse<TResponse>>> cacheService)
        {
            this.decoratee = decoratee;
            this.cacheService = cacheService;
        }

        public async Task<OrchestratorResponse<TResponse>> Orchestrate(OrchestratorRequest<TRequest> request)
        {
            IDictionary<TRequest, OrchestratorResponse<TResponse>> result = await cacheService.GetOrCreateAsync(request.CorrelationId, async () =>
            {
                OrchestratorResponse<TResponse> response = await decoratee.Orchestrate(request);

                return new Dictionary<TRequest, OrchestratorResponse<TResponse>>() { { request.Payload, response } };
            });

            return result.Values.FirstOrDefault();
        }
    }
}
