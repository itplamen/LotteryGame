namespace LotteryGame.Orchestrators.Api.IoC
{
    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Orchestrators.Cache.Contracts;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.DrawParticipation;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using LotteryGame.Orchestrators.Models.ReserveFunds;

    public sealed class OrchestratorsPackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<AvailableDrawOrchestrator>();
            services.AddTransient<DrawParticipationOrchestrator>();
            services.AddTransient<PurchaseTicketsOrchestrator>();
            services.AddTransient<ReserveFunsOrchestrator>();

            RegisterDecorator<AvailableDrawRequest, AvailableDrawResponse, AvailableDrawOrchestrator>(services);
            RegisterDecorator<DrawParticipationRequest, DrawParticipationResponse, DrawParticipationOrchestrator>(services);
            RegisterDecorator<PurchaseTicketsRequest, PurchaseTicketsResponse, PurchaseTicketsOrchestrator>(services);
            RegisterDecorator<ReserveFundsRequest, ReserveFundsResponse, ReserveFunsOrchestrator>(services);
        }

        private void RegisterDecorator<TRequest, TResponse, TOrchestrator>(IServiceCollection services)
            where TRequest : class
            where TResponse : class
            where TOrchestrator : IOrchestrator<TRequest, TResponse>
        {
            services.AddTransient<IOrchestrator<TRequest, TResponse>>(x =>
            {
                var inner = x.GetRequiredService<TOrchestrator>();
                var cache = x.GetRequiredService<ICacheService<IDictionary<TRequest, OrchestratorResponse<TResponse>>>>();

                return new LoggingOrchestratorDecorator<TRequest, TResponse>(inner, cache);
            });
        }
    }
}
