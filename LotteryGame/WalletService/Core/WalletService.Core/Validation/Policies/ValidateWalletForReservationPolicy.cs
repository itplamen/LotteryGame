namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class ValidateWalletForReservationPolicy : IOperationPolicy<ReservationOperationContext>
    {
        private readonly IRepository<Wallet> walletRepo;

        public ValidateWalletForReservationPolicy(IRepository<Wallet> walletRepo)
        {
            this.walletRepo = walletRepo;
        }

        public async Task<ResponseDto> ExecuteAsync(ReservationOperationContext context)
        {
            context.Wallet = await walletRepo.GetByIdAsync(context.Reservation.WalletId);
            if (context.Wallet == null)
            {
                return new ResponseDto("Wallet not found");
            }

            return new ResponseDto();
        }
    }
}
