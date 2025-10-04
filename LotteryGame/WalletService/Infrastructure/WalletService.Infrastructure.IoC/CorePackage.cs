namespace WalletService.Infrastructure.IoC
{
    using Microsoft.Extensions.DependencyInjection;

    using LotteryGame.Common.Models.IoC;
    
    using WalletService.Core.Contracts;
    using WalletService.Core.Operations;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IBalanceHistoryOperations, BalanceHistoryOperations>();
            services.AddScoped<IFundsOperations, FundsOperations>();
        }
    }
}
