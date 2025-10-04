namespace WagerService.Core.Contracts
{
    using LotteryGame.Common.Models.Dto;
    using WagerService.Core.Models;

    public interface ITicketOperations
    {
        Task<ResponseDto<IEnumerable<TicketDto>>> Create(TicketCreateRequestDto request);

        Task<ResponseDto<IEnumerable<TicketDto>>> Update(TicketUpdateRequestDto request);
    }
}
