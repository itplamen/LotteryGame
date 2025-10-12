namespace LotteryGame.Orchestrators.Api.IoC
{
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;

    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Orchestrators.Cache;
    using LotteryGame.Orchestrators.Cache.Contracts;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.Cache;
    using LotteryGame.Orchestrators.Models.DrawParticipation;
    using LotteryGame.Orchestrators.Models.PlayerProfile;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using LotteryGame.Orchestrators.Models.ReserveFunds;
    using LotteryGame.Orchestrators.Models.DrawHistory;

    public sealed class CachePackage : IPackage
    {
        private bool useInMemoryCache;
        private int absoluteExpiration;
        private readonly IConfiguration configuration;

        public CachePackage(IConfiguration configuration)
        {
            this.configuration = configuration;
            useInMemoryCache = bool.Parse(configuration["Cache:UseInMemoryCache"]);
            absoluteExpiration = int.Parse(configuration["Cache:AbsoluteExpiration"]);
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddMemoryCache();

            RegisterCacheService<OrchestratorCacheEntry<AvailableDrawRequest, AvailableDrawResponse>>(services);
            RegisterCacheService<OrchestratorCacheEntry<DrawParticipationRequest, DrawParticipationResponse>>(services);
            RegisterCacheService<OrchestratorCacheEntry<DrawHistoryRequest, DrawHistoryResponse>>(services);
            RegisterCacheService<OrchestratorCacheEntry<PlayerProfileRequest, PlayerProfileResponse>>(services);
            RegisterCacheService<OrchestratorCacheEntry<PurchaseTicketsRequest, PurchaseTicketsResponse>>(services);
            RegisterCacheService<OrchestratorCacheEntry<ReserveFundsRequest, ReserveFundsResponse>>(services);

            if (!useInMemoryCache)
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.InstanceName = "LotteryGameDb";
                    options.Configuration = configuration["Cache:ConnectionStrings:Redis"];
                });
            }
        }

        private void RegisterCacheService<TValue>(IServiceCollection services)
            where TValue : class
        {
            if (useInMemoryCache)
            {
                services.AddSingleton<ICacheService<TValue>, InMemoryCacheService<TValue>>(x =>
                new InMemoryCacheService<TValue>(
                    x.GetRequiredService<IMemoryCache>(),
                    absoluteExpiration));
            }
            else
            {
                services.AddTransient<ICacheService<TValue>, DistributedCacheService<TValue>>(x =>
                new DistributedCacheService<TValue>(
                    x.GetRequiredService<IDistributedCache>(),
                    absoluteExpiration));
            }
        }
    }
}
