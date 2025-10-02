namespace WagerService.Core.Contracts
{
    using WagerService.Core.Models;
    using WagerService.Data.Models;

    public interface ITicketOperations
    {
        Task<ResponseDto<TicketDto>> Create(int playerId, string drawId, long amount, int reservationId);

        Task<ResponseDto<TicketDto>> Update(string ticketId, TicketStatus status);
    }
}
