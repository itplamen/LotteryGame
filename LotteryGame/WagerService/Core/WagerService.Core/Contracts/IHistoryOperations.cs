namespace WagerService.Core.Contracts
{
    using LotteryGame.Common.Models.Dto;
    using WagerService.Core.Models;

    public interface IHistoryOperations
    {
        Task<ResponseDto<IEnumerable<TicketDto>>> GetHistory(IEnumerable<string> ticketIds);
    }
}
