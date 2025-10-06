namespace LotteryGame.Orchestrators.Contracts
{
    using LotteryGame.Orchestrators.Models.Base;

    public interface IOrchestrator<TRequest, TResponse>
    {
        Task<OrchestratorResponse<TResponse>> Orchestrate(OrchestratorRequest<TRequest> request);
    }
}
