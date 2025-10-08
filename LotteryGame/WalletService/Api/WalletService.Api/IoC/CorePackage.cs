namespace WalletService.Api.IoC
{
    using LotteryGame.Common.Models.IoC;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Contracts;
    using WalletService.Core.Operations;
    using WalletService.Core.Validation.Contexts;
    using WalletService.Core.Validation.Policies;

    public sealed class CorePackage : IPackage
    {
        public void RegisterServices(IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IBalanceHistoryOperations, BalanceHistoryOperations>();
            services.AddScoped<IFundsOperations, FundsOperations>();

            services.AddScoped<IOperationPolicy<WalletOperationContext>, ValidatePlayerIdPolicy>();
            services.AddScoped<IOperationPolicy<WalletOperationContext>, ValidateWalletExistsPolicy>();
            services.AddScoped<IOperationPolicy<WalletOperationContext>, ValidateSufficientFundsPolicy>();

            services.AddScoped<IOperationPolicy<ReservationOperationContext>, ValidateReservationExistsPolicy>();
            services.AddScoped<IOperationPolicy<ReservationOperationContext>, ValidateWalletForReservationPolicy>();
            services.AddScoped<IOperationPolicy<ReservationOperationContext>, ValidateReservationNotCapturedPolicy>();
            services.AddScoped<IOperationPolicy<ReservationOperationContext>, ValidateReservationNotExpiredPolicy>();
            services.AddScoped<IOperationPolicy<ReservationOperationContext>, ValidateReservationNotRefundedPolicy>();
            services.AddScoped<IOperationPolicy<ReservationOperationContext>, ValidateRefundableFundsPolicy>();

            services.AddScoped<OperationPipeline<WalletOperationContext>>();
            services.AddScoped<OperationPipeline<ReservationOperationContext>>();
        }
    }
}
