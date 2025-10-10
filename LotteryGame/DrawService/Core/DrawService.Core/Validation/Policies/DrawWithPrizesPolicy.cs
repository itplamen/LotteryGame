namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawWithPrizesPolicy : IOperationPolicy<GetPrizeOperationContext>
    {
        public Task<ResponseDto> ExecuteAsync(GetPrizeOperationContext context)
        {
            if (context.Draw.PrizeIds == null || !context.Draw.PrizeIds.Any())
            {
                return Task.FromResult(new ResponseDto("No prizes for the draw"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
