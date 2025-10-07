namespace LotteryGame.Orchestrators.Cache
{
    using Microsoft.Extensions.Caching.Distributed;
    
    using Newtonsoft.Json;

    using LotteryGame.Orchestrators.Cache.Contracts;

    public class DistributedCacheService<TValue> : ICacheService<TValue>
        where TValue : class
    {
        private readonly IDistributedCache cache;
        private readonly DistributedCacheEntryOptions options;

        public DistributedCacheService(IDistributedCache cache, int expiration)
        {
            this.cache = cache;
            this.options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expiration);
        }

        public async Task<TValue> GetOrCreateAsync(string key, Func<Task<TValue>> factory)
        {
            string cacheData = await cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cacheData))
            {
                var cachedValue = JsonConvert.DeserializeObject<TValue>(cacheData);

                if (cachedValue != null)
                {
                    return cachedValue;
                }
            }

            var value = await factory();
            string serialized = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(key, serialized, options);

            return value;
        }
    }
}
