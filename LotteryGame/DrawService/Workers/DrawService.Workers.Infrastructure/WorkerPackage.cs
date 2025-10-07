namespace DrawService.Workers.Infrastructure
{
    using CouchDB.Driver.DependencyInjection;
    using DrawService.Core.Contracts;
    using DrawService.Core.Operations;
    using DrawService.Core.PrizeDeterminations;
    using DrawService.Data;
    using DrawService.Data.Contracts;
    using LotteryGame.Common.Models.IoC;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    public class WorkerPackage : IPackage
    {
        private readonly IConfiguration configuration;

        public WorkerPackage(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<IPrizeStrategy, GrandPrizeStrategy>();
            services.AddTransient<IPrizeStrategy, SecondTierPrizeStrategy>();
            services.AddTransient<IPrizeStrategy, ThirdTierPrizeStrategy>();
            services.AddTransient<IPrizeDeterminationStrategy, PrizeDeterminationStrategy>();

            services.AddScoped<IDrawOperations, DrawOperations>();
            services.AddScoped<IPrizeOperations, PrizeOperations>();

            services.AddCouchContext<DrawServiceDbContext>(builder =>
            {
                builder
                    .UseEndpoint(configuration["CouchDB:Endpoint"])
                    .EnsureDatabaseExists()
                    .UseBasicAuthentication(
                        username: configuration["CouchDB:Username"],
                        password: configuration["CouchDB:Password"]
                    );
            });

            services.AddScoped(typeof(IRepository<>), typeof(DbRepository<>));
        }
    }
}
