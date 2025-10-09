namespace LotteryGame.Orchestrators.Models.Cache
{
    using LotteryGame.Orchestrators.Models.Base;

    public class OrchestratorCacheEntry<TRequest, TResponse>
    {
        public TRequest Payload { get; set; }

        public OrchestratorResponse<TResponse> Response { get; set; }
    }
}
