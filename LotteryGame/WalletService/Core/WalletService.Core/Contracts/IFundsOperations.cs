namespace WalletService.Core.Contracts
{
    using LotteryGame.Common.Models.Dto;

    public interface IFundsOperations
    {
        Task<ResponseDto> HasEnoughFunds(int playerId, long costAmount);

        Task<ResponseDto<BaseDto>> Reserve(int playerId, long amount);

        Task<ResponseDto> Capture(int reservationId, string ticketId);

        Task<ResponseDto> Refund(int reservationId);
    }
}
