namespace LotteryGame.Clients.Core.Services.Contracts
{
    public interface ILotteryService<TRequest, TResponse>
        where TRequest : class
        where TResponse : class
    {
        Task<TResponse> Execute(TRequest request);       
    }
}
