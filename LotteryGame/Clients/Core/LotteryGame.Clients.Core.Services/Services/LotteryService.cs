namespace LotteryGame.Clients.Core.Services.Services
{
    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Formatters;
    using LotteryGame.Clients.Core.Services.Models;
    using LotteryGame.Orchestrators.Api.Models.Protos.PlayerProfile;
    using LotteryGame.Orchestrators.Api.Models.Protos.TicketPurchase;

    public class LotteryService : ILotteryService
    {
        private readonly PlayerProfile.PlayerProfileClient playerProfileClient;
        private readonly TicketPurchase.TicketPurchaseClient ticketPurchaseClient;

        public LotteryService(PlayerProfile.PlayerProfileClient playerProfileClient, TicketPurchase.TicketPurchaseClient ticketPurchaseClient)
        {
            this.playerProfileClient = playerProfileClient;
            this.ticketPurchaseClient = ticketPurchaseClient;
        }

        public async Task<PlayerProfileResponse> GetProfile(int playerId)
        {
            ProfileProtoRequest protoReqest = new ProfileProtoRequest() { PlayerId = playerId };
            ProfileProtoResponse protoResponse = await playerProfileClient.GetAsync(protoReqest);

            return new PlayerProfileResponse()
            {
                Success = protoResponse.Success,
                ErrorMsg = protoResponse.ErrorMsg,
                BonusBalance = MoneyFormatter.ToDecimal(protoResponse.BonusBalanceInCents),
                RealBalance = MoneyFormatter.ToDecimal(protoResponse.RealBalanceInCents),
                DrawOptions = new DrawOptions()
                {
                    MaxPlayersInDraw = protoResponse.DrawOptions.MaxPlayersInDraw,
                    MinPlayersInDraw = protoResponse.DrawOptions.MinPlayersInDraw,
                    MaxTicketsPerPlayer = protoResponse.DrawOptions.MaxTicketsPerPlayer,
                    MinTicketsPerPlayer = protoResponse.DrawOptions.MinTicketsPerPlayer,
                    TicketPrice = MoneyFormatter.ToDecimal(protoResponse.DrawOptions.TicketPriceInCents)
                }
            };
        }

        public async Task<PurchaseTicketsResponse> PurchaseTickets(int playerId, int numberOfTickets)
        {
            PurchaseProtoRequest protoRequest = new PurchaseProtoRequest() 
            { 
                PlayerId = playerId, 
                NumberOfTickets = numberOfTickets 
            };

            PurchaseProtoResponse protoResponse = await ticketPurchaseClient.PurchaseAsync(protoRequest);

            return new PurchaseTicketsResponse()
            {
                Success = protoResponse.Success,
                ErrorMsg = protoResponse.ErrorMsg,
                TotalCost = MoneyFormatter.ToDecimal(protoResponse.TotalCostInCents),
                TicketNumbers = protoResponse.TicketNumbers
            };
        }
    }
}
