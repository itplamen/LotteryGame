namespace LotteryGame.Orchestrators.Gateways
{
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Orchestrators.Gateways.Contracts;
    using WalletService.Api.Models.Protos.Funds;

    public class FundsGateway : BaseGateway, IFundsGateway
    {
        private readonly Funds.FundsClient fundsClient;

        public FundsGateway(Funds.FundsClient fundsClient, IConfiguration configuration)
            : base(configuration)
        {
            this.fundsClient = fundsClient;
        }

        public async Task<BaseResponse> HasEnoughFunds(int playerId, int numberOfTickets, long ticketPriceInCents)
        {
            EnoughFundsRequest enoughFundsRequest = new EnoughFundsRequest()
            {
                PlayerId = playerId,
                CostAmount = numberOfTickets * ticketPriceInCents
            };

            return await Execute(async () => await fundsClient.HasEnoughFundsAsync(enoughFundsRequest));
        }

        public async Task<ReserveResponse> ReserveFunds(int playerId, long costAmount)
        {
            ReserveRequest reserveRequest = new ReserveRequest()
            {
                PlayerId = playerId,
                Amount = costAmount
            };

            return await Execute(async () => await fundsClient.ReserveAsync(reserveRequest));
        }

        public async Task<BaseResponse> CaptureFunds(int reservationId)
        {
            FundsRequest request = new FundsRequest()
            {
                ReservationId = reservationId
            };

            return await Execute(async () => await fundsClient.CaptureAsync(request));
        }

        public async Task<BaseResponse> RefundFunds(int reservationId)
        {
            FundsRequest fundsRequest = new FundsRequest()
            {
                ReservationId = reservationId
            };

            return await Execute(async () => await fundsClient.RefundAsync(fundsRequest));
        }
    }
}
