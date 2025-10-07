namespace LotteryGame.Orchestrators.Gateways
{
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Orchestrators.Gateways.Contracts;
    using WalletService.Api.Models.Protos.Funds;

    public class WalletGateway : BaseGateway, IWalletGateway
    {
        private readonly Funds.FundsClient fundsClient;

        public WalletGateway(Funds.FundsClient fundsClient, IConfiguration configuration)
            : base(configuration)
        {
            this.fundsClient = fundsClient;
        }

        public async Task<BaseProtoResponse> HasEnoughFunds(int playerId, long cost)
        {
            EnoughFundsProtoRequest enoughFundsRequest = new EnoughFundsProtoRequest()
            {
                PlayerId = playerId,
                Cost = cost
            };

            return await Execute(async () => await fundsClient.HasEnoughFundsAsync(enoughFundsRequest));
        }

        public async Task<ReserveProtoResponse> ReserveFunds(int playerId, long amount)
        {
            ReserveProtoRequest reserveRequest = new ReserveProtoRequest()
            {
                PlayerId = playerId,
                Amount = amount
            };

            return await Execute(async () => await fundsClient.ReserveAsync(reserveRequest));
        }

        public async Task<BaseProtoResponse> CaptureFunds(int reservationId)
        {
            FundsProtoRequest request = new FundsProtoRequest()
            {
                ReservationId = reservationId
            };

            return await Execute(async () => await fundsClient.CaptureAsync(request));
        }

        public async Task<BaseProtoResponse> RefundFunds(int reservationId)
        {
            FundsProtoRequest fundsRequest = new FundsProtoRequest()
            {
                ReservationId = reservationId
            };

            return await Execute(async () => await fundsClient.RefundAsync(fundsRequest));
        }
    }
}
