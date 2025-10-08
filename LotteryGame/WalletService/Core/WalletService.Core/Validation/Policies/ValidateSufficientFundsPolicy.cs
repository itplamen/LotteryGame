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

            if (context.Wallet!.TotalBalance < context.Amount)
            {
                responseDto.ErrorMsg= "Insufficient funds";
            }

            return Task.FromResult(responseDto);
        }
    }
}
