namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawStartPolicy : IOperationPolicy<StartDrawOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(StartDrawOperationContext context)
        {
            if (context.Draw.PlayerTickets.Count < context.MinTicketsPerPlayer)
            {
                return Task.FromResult(new ResponseDto("Draw cannot be started"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
