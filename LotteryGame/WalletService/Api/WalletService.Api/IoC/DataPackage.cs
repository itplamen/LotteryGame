namespace WalletService.Api.IoC
{
    using LotteryGame.Common.Models.IoC;
    using WalletService.Data;
    using WalletService.Data.Contracts;

    public sealed class DataPackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
        }
    }
}
