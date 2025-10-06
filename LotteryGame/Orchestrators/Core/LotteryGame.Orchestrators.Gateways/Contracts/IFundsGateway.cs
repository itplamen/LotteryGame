namespace LotteryGame.Orchestrators.Gateways.Contracts
{
    using WalletService.Api.Models.Protos.Funds;

    public interface IFundsGateway
    {
        Task<BaseResponse> HasEnoughFunds(int playerId, int numberOfTickets, long ticketPriceInCents);

        Task<ReserveResponse> ReserveFunds(int playerId, long costAmount);

        Task<BaseResponse> CaptureFunds(int reservationId);

        Task<BaseResponse> RefundFunds(int reservationId);
    }
}
