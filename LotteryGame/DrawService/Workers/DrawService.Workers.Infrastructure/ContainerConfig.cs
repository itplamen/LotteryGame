namespace DrawService.Workers.Infrastructure
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using LotteryGame.Common.Models.IoC;
    
    public static class ContainerConfig
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            IPackage[] packages =
            {
                new WorkerPackage(configuration)
            };

            RegisterServices(services, packages);
        }

        private static void RegisterServices(IServiceCollection services, IPackage[] packages)
        {
            foreach (var package in packages)
            {
                package.RegisterServices(services);
            }
        }
    }
}
