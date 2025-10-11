namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;
    using WalletService.Data.Contracts;
    using WalletService.Data.Models;

    public class ValidateReservationExistsPolicy : IOperationPolicy<ReservationOperationContext>
    {
        private readonly IRepository<Reservation> reservationRepo;

        public ValidateReservationExistsPolicy(IRepository<Reservation> reservationRepo)
        {
            this.reservationRepo = reservationRepo;
        }

        public async Task<ResponseDto> ExecuteAsync(ReservationOperationContext context)
        {
            if (context.ReservationId <= 0)
            {
                return new ResponseDto("Reservation not found");
            }

            context.Reservation = await reservationRepo.GetByIdAsync(context.ReservationId);
            if (context.Reservation == null)
            {
                return new ResponseDto("Reservation not found");
            }

            context.Amount = context.Reservation.AmountInCents;
            return new ResponseDto();
        }
    }
}
