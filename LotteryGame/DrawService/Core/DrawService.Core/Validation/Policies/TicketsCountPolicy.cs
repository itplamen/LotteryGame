namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class TicketsCountPolicy : IOperationPolicy<DrawOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(DrawOperationContext context)
        {
            int count = context.TicketIds?.Count() ?? 0;
            if (count < context.MinTicketsPerPlayer || count > context.MaxTicketsPerPlayer)
            {
                return Task.FromResult(new ResponseDto($"Invalid number of tickets. Min: {context.MinTicketsPerPlayer}, Max: {context.MaxTicketsPerPlayer}"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
