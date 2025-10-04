namespace WalletService.Api.IoC
{
    using AutoMapper;
    
    using LotteryGame.Common.Models.IoC;
    using WalletService.Api.Mapping;
    using WalletService.Core.Contracts;
    using WalletService.Core.Operations;

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
