namespace DrawService.Api.IoC
{
    using DrawService.Core.Contracts;
    using DrawService.Core.Operations;
    using DrawService.Core.PrizeDeterminations;
    using DrawService.Core.Validation.Contexts;
    using DrawService.Core.Validation.Policies;
    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Common.Utils.Validation;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IPrizeStrategy, GrandPrizeStrategy>();
            services.AddTransient<IPrizeStrategy, SecondTierPrizeStrategy>();
            services.AddTransient<IPrizeStrategy, ThirdTierPrizeStrategy>();
            services.AddTransient<IPrizeDeterminationStrategy, PrizeDeterminationStrategy>();

            services.AddScoped<IOperationPolicy<PrizeOperationContext>, DrawMustExistPolicy<PrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<PrizeOperationContext>, DrawInValidStatusPolicy<PrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<PrizeOperationContext>, DrawHasTicketsPolicy>();

            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawMustExistPolicy<DrawOperationContext>>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawInValidStatusPolicy<DrawOperationContext>>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, PlayerNotAlreadyJoinedPolicy>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawCapacityPolicy>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, TicketsCountPolicy>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawStartPolicy>();

            services.AddScoped<OperationPipeline<PrizeOperationContext>>();
            services.AddScoped<OperationPipeline<DrawOperationContext>>();

            services.AddScoped<IDrawOperations, DrawOperations>();
            services.AddScoped<IPrizeOperations, PrizeOperations>();
        }
    }
}
