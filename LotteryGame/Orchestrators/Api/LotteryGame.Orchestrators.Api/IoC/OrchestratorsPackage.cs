namespace LotteryGame.Orchestrators.Api.IoC
{
    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.AvailableDraw;
    using LotteryGame.Orchestrators.Models.DrawParticipation;
    using LotteryGame.Orchestrators.Models.PurchaseTickets;
    using LotteryGame.Orchestrators.Models.ReserveFunds;

    public sealed class OrchestratorsPackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddTransient<IOrchestrator<AvailableDrawRequest, AvailableDrawResponse>, AvailableDrawOrchestrator>();
            services.AddTransient<IOrchestrator<DrawParticipationRequest, DrawParticipationResponse>, DrawParticipationOrchestrator>();
            services.AddTransient<IOrchestrator<PurchaseTicketsRequest, PurchaseTicketsResponse>, PurchaseTicketsOrchestrator>();
            services.AddTransient<IOrchestrator<ReserveFundsRequest, ReserveFundsResponse>, ReserveFunsOrchestrator>();
        }
    }
}
