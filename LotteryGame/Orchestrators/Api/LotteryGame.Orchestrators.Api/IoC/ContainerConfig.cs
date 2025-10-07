namespace LotteryGame.Orchestrators.Api.IoC
{
    using LotteryGame.Common.Models.IoC;

    public static class ContainerConfig
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            IPackage[] packages =
            {
                new GatewaysPackage(configuration),
                new OrchestratorsPackage(),
                new CachePackage(configuration)
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
