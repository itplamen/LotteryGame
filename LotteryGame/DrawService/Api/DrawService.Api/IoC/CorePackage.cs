namespace DrawService.Api.IoC
{
    using DrawService.Core.Contracts;
    using DrawService.Core.Operations;
    using DrawService.Core.PrizeDeterminations;
    using LotteryGame.Common.Models.IoC;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IPrizeStrategy, GrandPrizeStrategy>();
            services.AddTransient<IPrizeStrategy, SecondTierPrizeStrategy>();
            services.AddTransient<IPrizeStrategy, ThirdTierPrizeStrategy>();
            services.AddTransient<IPrizeDeterminationStrategy, PrizeDeterminationStrategy>();

            services.AddScoped<IDrawOperations, DrawOperations>();
            services.AddScoped<IPrizeOperations, PrizeOperations>();
        }
    }
}
