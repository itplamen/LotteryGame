namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawHasTicketsPolicy : IOperationPolicy<PrizeOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(PrizeOperationContext context)
        {
            if (context.Draw.PlayerTickets == null || !context.Draw.PlayerTickets.Any())
            {
                return Task.FromResult(new ResponseDto("No tickets for draw"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
