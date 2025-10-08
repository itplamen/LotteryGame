namespace LotteryGame.Common.Utils.Validation
{
    using LotteryGame.Common.Models.Dto;

    public class OperationPipeline<TContext>
    {
        private readonly IEnumerable<IOperationPolicy<TContext>> policies;

        public OperationPipeline(IEnumerable<IOperationPolicy<TContext>> policies)
        {
            this.policies = policies;
        }

        public async Task<ResponseDto> ExecuteAsync(TContext context)
        {
            foreach (var policy in policies)
            {
                var error = await policy.ExecuteAsync(context);
                if (!error.IsSuccess)
                {
                    return error;
                }
            }

            return new ResponseDto();
        }
    }
}
