namespace WalletService.Core.Contracts
{
    using LotteryGame.Common.Models.Dto;
    using WalletService.Core.Models;

    public interface IFundsOperations
    {
        Task<ResponseDto> HasEnoughFunds(int playerId, long cost);

        Task<ResponseDto<WalletDto>> GetFunds(int playerId);

        Task<ResponseDto<BaseDto>> Reserve(int playerId, long amount);

        Task<ResponseDto> Capture(int reservationId);

        Task<ResponseDto> Refund(int reservationId);
    }
}
