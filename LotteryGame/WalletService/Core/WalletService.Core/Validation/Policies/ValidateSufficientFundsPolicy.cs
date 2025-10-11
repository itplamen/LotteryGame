namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class ValidateSufficientFundsPolicy : IOperationPolicy<WalletOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(WalletOperationContext context)
        {
            ResponseDto responseDto = new ResponseDto();

            if (context.Amount.HasValue && context.Amount > context.Wallet.TotalBalanceInCents)
            {
                responseDto.ErrorMsg= "Insufficient funds";
            }

            return Task.FromResult(responseDto);
        }
    }
}
