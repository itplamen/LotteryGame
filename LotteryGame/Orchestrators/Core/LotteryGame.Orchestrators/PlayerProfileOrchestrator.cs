namespace LotteryGame.Orchestrators
{
    using AutoMapper;
 
    using DrawService.Api.Models.Protos.Draws;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Gateways.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.PlayerProfile;
    using WalletService.Api.Models.Protos.Funds;

    public class PlayerProfileOrchestrator : IOrchestrator<PlayerProfileRequest, PlayerProfileResponse>
    {
        private readonly IMapper mapper;
        private readonly IDrawGateway drawGateway;
        private readonly IWalletGateway walletGateway;

        public PlayerProfileOrchestrator(IMapper mapper, IDrawGateway drawGateway, IWalletGateway walletGateway)
        {
            this.mapper = mapper;
            this.drawGateway = drawGateway;
            this.walletGateway = walletGateway;
        }

        public async Task<OrchestratorResponse<PlayerProfileResponse>> Orchestrate(OrchestratorRequest<PlayerProfileRequest> request)
        {
            WalletProtoResponse walletResponse = await walletGateway.GetFunds(request.Payload.PlayerId);

            if (walletResponse.Success)
            {
                DrawOptionsProtoResponse drawOptions = await drawGateway.GetDrawOptions();

                return new OrchestratorResponse<PlayerProfileResponse>() 
                { 
                    Data = new PlayerProfileResponse()
                    {
                        BonusBalance = walletResponse.BonusMoney,
                        RealBalance = walletResponse.RealMoney,
                        DrawOptions = mapper.Map<DrawOptions>(drawOptions)
                    },
                    ErrorMsg = string.Empty,
                    Success = true
                };
            }

            return new OrchestratorResponse<PlayerProfileResponse>() { ErrorMsg = "Cound not fetch player profile data" };
        }
    }
}
