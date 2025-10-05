namespace WagerService.Api.Ioc
{
    using LotteryGame.Common.Models.IoC;
    using WagerService.Data;
    using WagerService.Data.Contracts;

    public sealed class DataPackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
        }
    }
}
