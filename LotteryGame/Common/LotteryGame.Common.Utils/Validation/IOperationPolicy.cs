namespace LotteryGame.Common.Utils.Validation
{
    using LotteryGame.Common.Models.Dto;

    public interface IOperationPolicy<TContext>
    {
        Task<ResponseDto> ExecuteAsync(TContext context);
    }
}
