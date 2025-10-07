namespace LotteryGame.Orchestrators.Cache
{
    using Microsoft.Extensions.Caching.Memory;

    using LotteryGame.Orchestrators.Cache.Contracts;
    
    public class InMemoryCacheService<TValue> : ICacheService<TValue>
        where TValue : class
    {
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions options;

        public InMemoryCacheService(IMemoryCache memoryCache, int expiration)
        {
            this.memoryCache = memoryCache;
            this.options = new MemoryCacheEntryOptions();
            options.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(expiration);
        }

        public async Task<TValue> GetOrCreateAsync(string key, Func<Task<TValue>> factory)
        {
            return await memoryCache.GetOrCreateAsync(key, async entry =>
            {
                return await factory();
            });
        }
    }
}
