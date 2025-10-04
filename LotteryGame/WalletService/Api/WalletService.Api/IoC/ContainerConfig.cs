namespace WalletService.Api.IoC
{
    using LotteryGame.Common.Models.IoC;
    public static class ContainerConfig
    {
        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            IPackage[] packages =
            {
                new DataPackage(),
                new CorePackage()
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
