namespace LotteryGame.Clients.Core.Services.Services
{
    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Formatters;
    using LotteryGame.Clients.Core.Services.Models.Betting;
    using LotteryGame.Orchestrators.Api.Models.Protos.TicketPurchase;

    public class LotteryBettingService : ILotteryService<BettingRequest, BettingResponse>
    {
        private readonly TicketPurchase.TicketPurchaseClient client;

        public LotteryBettingService(TicketPurchase.TicketPurchaseClient client)
        {
            this.client = client;
        }

        public async Task<BettingResponse> Execute(BettingRequest request)
        {
            PurchaseProtoRequest protoRequest = new PurchaseProtoRequest()
            {
                PlayerId = request.PlayerId,
                NumberOfTickets = request.NumberOfTickets
            };

            PurchaseProtoResponse protoResponse = await client.PurchaseAsync(protoRequest);

            return new BettingResponse()
            {
                Success = protoResponse.Success,
                ErrorMsg = protoResponse.ErrorMsg,
                TotalCost = MoneyFormatter.ToDecimal(protoResponse.TotalCostInCents),
                DrawId = protoResponse.DrawId,
                TicketNumbers = protoResponse.TicketNumbers,
                DrawDate = protoResponse.DrawDate?.ToDateTime() ?? default(DateTime)
            };
        }
    }
}
