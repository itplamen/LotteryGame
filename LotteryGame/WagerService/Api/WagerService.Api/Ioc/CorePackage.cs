namespace WagerService.Api.Ioc
{
    using Microsoft.Extensions.DependencyInjection;

    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Common.Utils.Validation;
    using WagerService.Core.Contracts;
    using WagerService.Core.NumberGenerators;
    using WagerService.Core.Operations;
    using WagerService.Core.Validation.Contexts;
    using WagerService.Core.Validation.Policies;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddSingleton<RandomNumberGeneration>();
            services.AddSingleton<SequentialNumberGeneration>();
            services.AddSingleton<SnowflakeNumberGenerator>();

            services.AddSingleton<INumberGeneration>(x =>
            {
                var config = x.GetRequiredService<IConfiguration>();
                var strategy = config["NumberGenerator"];

                return strategy switch
                {
                    "Random" => x.GetRequiredService<RandomNumberGeneration>(),
                    "Sequential" => x.GetRequiredService<SequentialNumberGeneration>(),
                    "Snowflake" => x.GetRequiredService<SnowflakeNumberGenerator>(),
                    _ => throw new InvalidOperationException("Invalid number generation strategy")
                };
            });

            services.AddScoped<IOperationPolicy<TicketOperationContext>, ValidateNumberOfTicketsPolicy>();
            services.AddScoped(x =>
                new OperationPipeline<TicketOperationContext>(
                    x.GetServices<IOperationPolicy<TicketOperationContext>>()
                )
            );

            services.AddScoped<IOperationPolicy<TicketOperationContext>, ValidateTicketsExistPolicy>();
            services.AddScoped(x =>
                new OperationPipeline<TicketOperationContext>(
                    x.GetServices<IOperationPolicy<TicketOperationContext>>()
                )
            );

            services.AddScoped<ITicketOperations, TicketOperations>();
        }
    }
}
