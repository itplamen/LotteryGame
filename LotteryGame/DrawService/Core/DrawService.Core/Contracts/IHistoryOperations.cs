namespace DrawService.Core.Contracts
{
    using DrawService.Core.Models;
    using LotteryGame.Common.Models.Dto;

    public interface IHistoryOperations
    {
        Task<ResponseDto<HistoryDto>> GetHistory(string drawId);
    }
}
