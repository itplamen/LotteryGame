namespace LotteryGame.Clients.Core.Services.Services
{
    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Formatters;
    using LotteryGame.Clients.Core.Services.Models.History;
    using LotteryGame.Orchestrators.Api.Models.Protos.LotteryHistory;

    public class LotteryHistoryService : ILotteryService<HistoryRequest, HistoryResponse>
    {
        private readonly LotteryHistory.LotteryHistoryClient client;

        public LotteryHistoryService(LotteryHistory.LotteryHistoryClient client)
        {
            this.client = client;
        }

        public async Task<HistoryResponse> Execute(HistoryRequest request)
        {
            HistoryProtoRequest protoRequest = new HistoryProtoRequest()
            {
                DrawId = request.DrawId,
            };

            HistoryProtoResponse protoResponse = await client.GetAsync(protoRequest);

            return new HistoryResponse()
            {
                Success = protoResponse.Success,
                ErrorMsg = protoResponse.ErrorMsg,
                DrawDate = protoResponse.DrawDate?.ToDateTime() ?? default(DateTime),
                DrawStatus = protoResponse.DrawStatus,
                HouseProfit = MoneyFormatter.ToDecimal(protoResponse.HouseProfitInCents),
                Participants = protoResponse.Participants,
                Prizes = protoResponse.Prizes?.Select(x => new PrizeHistory()
                {
                    Amount = MoneyFormatter.ToDecimal(x.AmountInCents),
                    Status = x.Status,
                    TicketNumber = x.TicketNumber,
                    Tier = x.Tier,
                    PlayerId = x.PlayerId
                })
            };
        }
    }
}
