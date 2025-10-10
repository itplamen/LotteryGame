namespace DrawService.Core.Contracts
{
    using DrawService.Core.Models;
    using LotteryGame.Common.Models.Dto;

    public interface IPrizeOperations
    {
        Task<ResponseDto<IEnumerable<PrizeDto>>> DeterminePrizes(string drawId);

        Task<ResponseDto<IEnumerable<PrizeDto>>> GetPrizes(string drawId);
    }
}
