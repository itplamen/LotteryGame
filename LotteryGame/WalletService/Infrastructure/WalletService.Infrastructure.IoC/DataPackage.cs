namespace WalletService.Infrastructure.IoC
{
    using Microsoft.Extensions.DependencyInjection;

    using LotteryGame.Common.Models.IoC;
    using WalletService.Data.Contracts;
    using WalletService.Data;

    public sealed class DataPackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
        }
    }
}
