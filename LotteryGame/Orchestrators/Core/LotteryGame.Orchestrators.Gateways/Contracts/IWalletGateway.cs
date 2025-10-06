namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using WalletService.Api.Models.Protos.Funds;

    public interface IWalletGateway
    {
        Task<BaseResponse> HasEnoughFunds(int playerId, long cost);

        Task<ReserveResponse> ReserveFunds(int playerId, long amount);

        Task<BaseResponse> CaptureFunds(int reservationId);

        Task<BaseResponse> RefundFunds(int reservationId);
    }
}
