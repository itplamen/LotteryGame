namespace DrawService.Workers.Infrastructure
{
    using CouchDB.Driver.DependencyInjection;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using DrawService.Core.Contracts;
    using DrawService.Core.Operations;
    using DrawService.Core.PrizeDeterminations;
    using DrawService.Core.Validation.Contexts;
    using DrawService.Core.Validation.Policies;
    using DrawService.Data;
    using DrawService.Data.Contracts;
    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Common.Utils.Validation;
  
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

            services.AddScoped<IOperationPolicy<PrizeOperationContext>, DrawMustExistPolicy<PrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<PrizeOperationContext>, DrawInValidStatusPolicy<PrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<PrizeOperationContext>, DrawHasTicketsPolicy>();

            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawMustExistPolicy<DrawOperationContext>>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawInValidStatusPolicy<DrawOperationContext>>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, PlayerNotAlreadyJoinedPolicy>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawCapacityPolicy>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, TicketsCountPolicy>();
            services.AddScoped<IOperationPolicy<DrawOperationContext>, DrawStartPolicy>();

            services.AddScoped<OperationPipeline<PrizeOperationContext>>();
            services.AddScoped<OperationPipeline<DrawOperationContext>>();

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
