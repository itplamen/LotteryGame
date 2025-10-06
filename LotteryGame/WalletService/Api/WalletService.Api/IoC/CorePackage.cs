namespace WalletService.Api.IoC
{
    using LotteryGame.Common.Models.IoC;
    using WalletService.Core.Contracts;
    using WalletService.Core.Operations;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IBalanceHistoryOperations, BalanceHistoryOperations>();
            services.AddScoped<IFundsOperations, FundsOperations>();
        }
    }
}
