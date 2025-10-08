namespace WalletService.Core.Validation.Policies
{
    using Microsoft.EntityFrameworkCore;

    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;
    using LotteryGame.Common.Models.Dto;

    public class ValidateWalletExistsPolicy : IOperationPolicy<WalletOperationContext>
    {
        private readonly IRepository<Wallet> walletRepo;

        public ValidateWalletExistsPolicy(IRepository<Wallet> walletRepo)
        {
            this.walletRepo = walletRepo;
        }

        public async Task<ResponseDto> ExecuteAsync(WalletOperationContext context)
        {
            Wallet wallet = await walletRepo.Filter().FirstOrDefaultAsync(x => x.PlayerId == context.PlayerId);
            if (wallet == null)
            {
                return new ResponseDto("Wallet not found");
            }
            
            context.Wallet = wallet;
            return new ResponseDto();
        }
    }
}
