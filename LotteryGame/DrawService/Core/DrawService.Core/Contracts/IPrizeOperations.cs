namespace DrawService.Core.Contracts
{
    using DrawService.Core.Models;

    public interface IPrizeOperations
    {
        Task<ResponseDto<IEnumerable<PrizeDto>>> DeterminePrizes(string drawId);
    }
}
