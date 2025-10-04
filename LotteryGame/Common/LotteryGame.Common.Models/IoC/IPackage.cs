namespace LotteryGame.Common.Models.IoC
{
    using Microsoft.Extensions.DependencyInjection;

    public interface IPackage
    {
        void RegisterServices(IServiceCollection services);
    }
}
