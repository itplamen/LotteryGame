namespace WalletService.Core.Contracts
{
    using WalletService.Core.Models;

    public interface IFundsOperations
    {
        Task<ResponseDto> HasEnoughFunds(int playerId, long costAmount);

        Task<ResponseDto<BaseDto>> Reserve(int playerId, long amount, string ticketId);

        Task<ResponseDto> Capture(int reservationId);

        Task<ResponseDto> Refund(int reservationId);
    }
}
