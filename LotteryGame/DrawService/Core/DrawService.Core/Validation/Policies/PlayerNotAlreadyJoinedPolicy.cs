namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class PlayerNotAlreadyJoinedPolicy : IOperationPolicy<DrawOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(DrawOperationContext context)
        {
            if (context.Join && context.Draw.PlayerTickets.ContainsKey(context.PlayerId))
            {
                return Task.FromResult(new ResponseDto("Player already joined the draw"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
