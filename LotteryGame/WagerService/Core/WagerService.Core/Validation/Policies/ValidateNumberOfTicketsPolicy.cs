namespace WagerService.Core.Validation.Policies
{
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;
    using WagerService.Core.Validation.Contexts;

    public class ValidateNumberOfTicketsPolicy : IOperationPolicy<TicketOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(TicketOperationContext context)
        {
            if (context.NumberOfTickets < 0)
            {
                return Task.FromResult(new ResponseDto { ErrorMsg = "Invalid number of tickets to create" });
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
