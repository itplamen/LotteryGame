namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class PlayerNotAlreadyJoinedPolicy : IOperationPolicy<JoinDrawOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(JoinDrawOperationContext context)
        {
            if (context.Draw.PlayerTickets.ContainsKey(context.PlayerId))
            {
                return Task.FromResult(new ResponseDto("Player already joined the draw"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
