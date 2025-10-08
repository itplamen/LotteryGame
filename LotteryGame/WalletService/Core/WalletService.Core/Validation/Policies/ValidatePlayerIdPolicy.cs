namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class ValidatePlayerIdPolicy : IOperationPolicy<WalletOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(WalletOperationContext context)
        {
            return Task.FromResult(new ResponseDto(context.PlayerId <= 0 ? "Invalid player Id" : null));
        }
    }
}
