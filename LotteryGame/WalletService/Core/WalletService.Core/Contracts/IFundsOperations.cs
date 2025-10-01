namespace WalletService.Core.Contracts
{
    using WalletService.Core.Models;

    public interface IFundsOperations
    {
        Task<ResponseDto<BaseDto>> Reserve(int playerId, decimal amount, string ticketId);

        Task<ResponseDto> Capture(int reservationId);

        Task<ResponseDto> Refund(int reservationId);
    }
}
