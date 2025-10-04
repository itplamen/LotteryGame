namespace WalletService.Infrastructure.IoC
{
    using AutoMapper;

    using Microsoft.Extensions.DependencyInjection;

    using LotteryGame.Common.Models.IoC;
    using WalletService.Core.Contracts;
    using WalletService.Core.Operations;
    using WalletService.Infrastructure.Mapping;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(x => x.AddProfiles(new List<Profile>() { new DataProfile(), new ProtosProfile() }));
            services.AddScoped<IBalanceHistoryOperations, BalanceHistoryOperations>();
            services.AddScoped<IFundsOperations, FundsOperations>();
        }
    }
}
