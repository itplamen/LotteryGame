namespace LotteryGame.Clients.Core.Services.Managers
{
    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Models.Betting;
    using LotteryGame.Clients.Core.Services.Models.History;
    using LotteryGame.Clients.Core.Services.Models.Profile;
    using LotteryGame.Clients.Core.Wrapper.Contracts;

    public class LotteryManager : ILotteryManager
    {
        private const int POOL_MAX_RETRIES = 20;
        private const int POOL_DELAY_MS = 1000;

        private readonly IClientManager clientManager;
        private readonly ILotteryService<ProfileRequest, ProfileResponse> profileService;
        private readonly ILotteryService<BettingRequest, BettingResponse> bettingService;
        private readonly ILotteryService<HistoryRequest, HistoryResponse> historyService;

        public LotteryManager(
            IClientManager clientManager,
            ILotteryService<ProfileRequest, ProfileResponse> profileService,
            ILotteryService<BettingRequest, BettingResponse> bettingService,
            ILotteryService<HistoryRequest, HistoryResponse> historyService)
        {
            this.clientManager = clientManager;
            this.profileService = profileService;
            this.bettingService = bettingService;
            this.historyService = historyService;
        }

        public async Task<ProfileResponse> GetProfile(int playerId)
        {
            ProfileResponse response = await profileService.Execute(new ProfileRequest(playerId));

            if (!response.Success)
            {
                return new ProfileResponse()
                {
                    Success = false,
                    ErrorMsg = $"{response.ErrorMsg}"
                };
            }

            return response;
        }

        public async Task<BettingResponse> PLayerPurchase(int playerId, int minTickets, int maxTickets)
        {
            string input = clientManager.ReadLine();

            if (!int.TryParse(input, out int numberOfTickets))
            {
                return new BettingResponse() { Success = false, ErrorMsg = "Input is not a valid number." };
            }

            if (numberOfTickets < minTickets || numberOfTickets > maxTickets)
            {
                return new BettingResponse() { Success = false, ErrorMsg = $"Ticket count must be between {minTickets} and {maxTickets}." };
            }

            BettingResponse response = await bettingService.Execute(new BettingRequest(playerId, numberOfTickets));

            if (!response.Success)
            {
                return new BettingResponse() { Success = false, ErrorMsg = $"{response.ErrorMsg}" };
            }

            return response;
        }

        public async Task<IEnumerable<BettingResponse>> CpuPurchase(int cpuStartId, int cpuCount, int numberOfTickets)
        {
            Task<BettingResponse>[] tasks = Enumerable.Range(cpuStartId, cpuCount)
                .Select(id => Task.Run(() => bettingService.Execute(new BettingRequest(id, numberOfTickets))))
                .ToArray();

            return await Task.WhenAll(tasks);
        }

        public async Task<HistoryResponse> WaitForDraw(string drawId)
        {
            int retries = 0;

            while (retries < POOL_MAX_RETRIES)
            {
                retries++;

                HistoryResponse response = await historyService.Execute(new HistoryRequest(drawId));

                if (response.Success)
                {
                    return response;
                }

                await Task.Delay(POOL_DELAY_MS);
            }

            return new HistoryResponse()
            {
                Success = false,
                ErrorMsg = "Polling stopped: max retries reached."
            };
        }
    }
}