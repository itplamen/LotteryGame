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

            services.AddScoped<IOperationPolicy<GetPrizeOperationContext>, DrawMustExistPolicy<GetPrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<GetPrizeOperationContext>, DrawInValidStatusPolicy<GetPrizeOperationContext>>();
            services.AddScoped<IOperationPolicy<GetPrizeOperationContext>, DrawWithPrizesPolicy>();

            services.AddScoped<IOperationPolicy<StartDrawOperationContext>, DrawMustExistPolicy<StartDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<StartDrawOperationContext>, DrawInValidStatusPolicy<StartDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<StartDrawOperationContext>, DrawStartPolicy>();

            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, DrawMustExistPolicy<JoinDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, DrawInValidStatusPolicy<JoinDrawOperationContext>>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, PlayerNotAlreadyJoinedPolicy>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, DrawCapacityPolicy>();
            services.AddScoped<IOperationPolicy<JoinDrawOperationContext>, TicketsCountPolicy>();

            services.AddScoped<OperationPipeline<PrizeOperationContext>>();
            services.AddScoped<OperationPipeline<StartDrawOperationContext>>();
            services.AddScoped<OperationPipeline<JoinDrawOperationContext>>();
            services.AddScoped<OperationPipeline<GetPrizeOperationContext>>();

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
