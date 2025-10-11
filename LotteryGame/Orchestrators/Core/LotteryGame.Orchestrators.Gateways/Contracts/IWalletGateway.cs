namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using WalletService.Api.Models.Protos.Funds;

    public interface IWalletGateway
    {
        Task<BaseProtoResponse> HasEnoughFunds(int playerId, long cost);

        Task<WalletProtoResponse> GetFunds(int playerId);

        Task<ReserveProtoResponse> ReserveFunds(int playerId, long amount);

        Task<BaseProtoResponse> CaptureFunds(int reservationId);

        Task<BaseProtoResponse> RefundFunds(int reservationId);
    }
}
