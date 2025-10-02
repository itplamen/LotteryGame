namespace WagerService.Core.Contracts
{
    using WagerService.Core.Models;

    public interface ITicketOperations
    {
        Task<ResponseDto<TicketDto>> Create(TicketCreateRequestDto request);

        Task<ResponseDto<TicketDto>> Update(TicketUpdateRequestDto request);
    }
}
