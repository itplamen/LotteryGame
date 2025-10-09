namespace DrawService.Core.Validation.Policies
{
    using DrawService.Core.Validation.Contexts;
    using LotteryGame.Common.Models.Dto;
    using LotteryGame.Common.Utils.Validation;

    public class DrawInValidStatusPolicy<TContext> : IOperationPolicy<TContext>
        where TContext : BaseDrawContext
    {
        public Task<ResponseDto> ExecuteAsync(TContext context)
        {
            if (context.Draw.Status != context.Status)
            {
                return Task.FromResult(new ResponseDto("Invalid draw status"));
            }

            return Task.FromResult(new ResponseDto());
        }
    }
}
