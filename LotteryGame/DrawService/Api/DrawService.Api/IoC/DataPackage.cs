namespace DrawService.Api.IoC
{
    using CouchDB.Driver.DependencyInjection;
    
    using DrawService.Data;
    using DrawService.Data.Contracts;
    using DrawService.Data.Models;
    using LotteryGame.Common.Models.IoC;

    public sealed class DataPackage : IPackage
    {
        private readonly IConfiguration configuration;

        public DataPackage(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void RegisterServices(IServiceCollection services)
        {
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

            services.AddSingleton<IConflictResolver<Draw>, DrawConflictResolver>();
            services.AddSingleton<IConflictResolver<Prize>, PrizeConflictResolver>();
        }
    }
}
