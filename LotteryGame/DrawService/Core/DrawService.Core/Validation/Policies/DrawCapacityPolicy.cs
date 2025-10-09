namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawCapacityPolicy : IOperationPolicy<DrawOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(DrawOperationContext context)
        {
            if (context.Join && context.Draw.PlayerTickets.Count >= context.Draw.MaxPlayersInDraw)
            {
                return Task.FromResult(new ResponseDto("Draw is full"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
