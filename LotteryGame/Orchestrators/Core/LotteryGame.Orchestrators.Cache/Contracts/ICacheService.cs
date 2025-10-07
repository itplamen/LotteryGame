namespace LotteryGame.Orchestrators.Cache.Contracts
{
    public interface ICacheService<TValue>
        where TValue : class
    {
        Task<TValue> GetOrCreateAsync(string key, Func<Task<TValue>> factory);
    }
}
