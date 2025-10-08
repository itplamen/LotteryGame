namespace WalletService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WalletService.Core.Validation.Contexts;

    public class ValidateReservationNotRefundedPolicy : IOperationPolicy<ReservationOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(ReservationOperationContext context)
        {
            if (context.Reservation.DeletedOn.HasValue)
            {
                return Task.FromResult(new ResponseDto("Reservation already refunded"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
