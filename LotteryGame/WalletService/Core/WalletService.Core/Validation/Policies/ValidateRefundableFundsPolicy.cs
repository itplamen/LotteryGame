namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class ValidateRefundableFundsPolicy : IOperationPolicy<ReservationOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(ReservationOperationContext context)
        {
            return Task.FromResult(new ResponseDto(
                context.Wallet.LockedFundsInCents < context.Amount ? 
                "Insufficient locked funds to process refund" : 
                null));
        }
    }
}
