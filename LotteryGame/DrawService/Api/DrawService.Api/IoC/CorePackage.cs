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

            services.AddScoped<IOperationPolicy<GetPrizeOperationContext>, DrawMustExistPolicy<GetPrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<GetPrizeOperationContext>, DrawInValidStatusPolicy<GetPrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<GetPrizeOperationContext>, DrawWithPrizesPolicy>();

            services.AddScoped<IOperationPolicy<StartDrawOperationContext>, DrawMustExistPolicy<StartDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<StartDrawOperationContext>, DrawInValidStatusPolicy<StartDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<StartDrawOperationContext>, DrawStartPolicy>();

            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, DrawMustExistPolicy<JoinDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, DrawInValidStatusPolicy<JoinDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, PlayerNotAlreadyJoinedPolicy>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, DrawCapacityPolicy>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, TicketsCountPolicy>();

            services.AddScoped<IOperationPolicy<HistoryOperationContext>, DrawMustExistPolicy<HistoryOperationContext>>();
            services.AddScoped<IOperationPolicy<HistoryOperationContext>, DrawInValidStatusPolicy<HistoryOperationContext>>();

            services.AddScoped<OperationPipeline<PrizeOperationContext>>();
            services.AddScoped<OperationPipeline<StartDrawOperationContext>>();
            services.AddScoped<OperationPipeline<JoinDrawOperationContext>>();
            services.AddScoped<OperationPipeline<GetPrizeOperationContext>>();
            services.AddScoped<OperationPipeline<HistoryOperationContext>>();

            services.AddScoped<IDrawOperations, DrawOperations>();
            services.AddScoped<IPrizeOperations, PrizeOperations>();
            services.AddScoped<IHistoryOperations, HistoryOperations>();
        }
    }
}
