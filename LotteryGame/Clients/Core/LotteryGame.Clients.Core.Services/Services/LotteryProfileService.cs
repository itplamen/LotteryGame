namespace LotteryGame.Clients.Core.Services.Services
{
    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Formatters;
    using LotteryGame.Clients.Core.Services.Models.Profile;
    using LotteryGame.Orchestrators.Api.Models.Protos.PlayerProfile;

    public class LotteryProfileService : ILotteryService<ProfileRequest, ProfileResponse>
    {
        private readonly PlayerProfile.PlayerProfileClient client;

        public LotteryProfileService(PlayerProfile.PlayerProfileClient client)
        {
            this.client = client;
        }

        public async Task<ProfileResponse> Execute(ProfileRequest request)
        {
            ProfileProtoRequest protoReqest = new ProfileProtoRequest() { PlayerId = request.PlayerId };
            ProfileProtoResponse protoResponse = await client.GetAsync(protoReqest);

            return new ProfileResponse()
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
    }
}
