namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class ValidateReservationNotCapturedPolicy : IOperationPolicy<ReservationOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(ReservationOperationContext context)
        {
            return Task.FromResult(new ResponseDto(context.Reservation.IsCaptured ? "Reservation already captured. Contact custommer support." : null));
        }
    }
}
