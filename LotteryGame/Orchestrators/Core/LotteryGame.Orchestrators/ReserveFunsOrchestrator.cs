namespace LotteryGame.Orchestrators
{
    using AutoMapper;
    
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.ReserveFunds;
    using WalletService.Api.Models.Protos.Funds;

    public class ReserveFunsOrchestrator : IOrchestrator<ReserveFundsRequest, ReserveFundsResponse>
    {
        private readonly IMapper mapper;
        private readonly IWalletGateway walletGateway;

        public ReserveFunsOrchestrator(IMapper mapper, IWalletGateway walletGateway)
        {
            this.mapper = mapper;
            this.walletGateway = walletGateway;
        }

        public async Task<OrchestratorResponse<ReserveFundsResponse>> Orchestrate(OrchestratorRequest<ReserveFundsRequest> request)
        {
            long cost = request.Payload.NumberOfTickets * request.Payload.TicketPriceInCents;
            BaseProtoResponse hasFunds = await walletGateway.HasEnoughFunds(request.Payload.PlayerId, cost);

            if (!hasFunds.Success)
            {
                return mapper.Map<OrchestratorResponse<ReserveFundsResponse>>(hasFunds);
            }

            ReserveProtoResponse reserveResponse = await walletGateway.ReserveFunds(request.Payload.PlayerId, cost);
            return mapper.Map<OrchestratorResponse<ReserveFundsResponse>>(reserveResponse);
        }
    }
}
