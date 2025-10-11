namespace LotteryGame.Orchestrators.Api.Services
{
    using AutoMapper;
    
    using Grpc.Core;

    using LotteryGame.Orchestrators.Api.Models.Protos.PlayerProfile;
    using LotteryGame.Orchestrators.Contracts;
    using LotteryGame.Orchestrators.Models.Base;
    using LotteryGame.Orchestrators.Models.PlayerProfile;

    public class PlayerProfileService : PlayerProfile.PlayerProfileBase
    {
        private readonly IMapper mapper;
        private readonly IOrchestrator<PlayerProfileRequest, PlayerProfileResponse> orchestrator;

        public PlayerProfileService(IMapper mapper, IOrchestrator<PlayerProfileRequest, PlayerProfileResponse> orchestrator)
        {
            this.mapper = mapper;
            this.orchestrator = orchestrator;
        }

        public override async Task<ProfileProtoResponse> Get(ProfileProtoRequest request, ServerCallContext context)
        {
            OrchestratorRequest<PlayerProfileRequest> profileRequest = new(new PlayerProfileRequest { PlayerId = request.PlayerId });
            OrchestratorResponse<PlayerProfileResponse> profileResponse = await orchestrator.Orchestrate(profileRequest);

            return mapper.Map<ProfileProtoResponse>(profileResponse);
        }
    }
}
