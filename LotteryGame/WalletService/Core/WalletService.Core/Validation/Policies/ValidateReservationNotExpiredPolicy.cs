namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class ValidateReservationNotExpiredPolicy : IOperationPolicy<ReservationOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(ReservationOperationContext context)
        {
            if (context.Reservation == null)
            {
                return Task.FromResult(new ResponseDto("Reservation not found"));
            }

            if (context.Reservation.ExpiresAt < DateTime.UtcNow)
            {
                return Task.FromResult(new ResponseDto("Reservation expired"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
