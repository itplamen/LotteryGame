namespace LotteryGame.Clients.UI.ConsoleApp
{
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Wrapper.Contracts;
    
    public class ConsoleProgramExecutor : IProgramExecutor
    {
        private readonly int playerId;
        private readonly int cpuPlayers;
        private readonly int cpuPlayerTickets;
        private readonly ILotteryManager lotteryManager;

        public ConsoleProgramExecutor(ILotteryManager lotteryManager, IConfiguration configuration)
        {
            playerId = int.Parse(configuration["PlayerId"]);
            cpuPlayers = int.Parse(configuration["CpuPlayers"]);
            cpuPlayerTickets = int.Parse(configuration["CpuPlayerTickets"]);

            this.lotteryManager = lotteryManager;
        }

        public async Task Run()
        {
            Console.WriteLine("Starting Lottery Game...");

            var profile = await lotteryManager.GetProfile(playerId);

            if (!profile.Success)
            {
                return;             
            }

            var purchase = await lotteryManager.PLayerPurchase(playerId, profile.DrawOptions.MinTicketsPerPlayer, profile.DrawOptions.MaxTicketsPerPlayer);

            if (purchase.Success)
            {
                await lotteryManager.CpuPurchase(playerId + 1, cpuPlayers, cpuPlayerTickets);
                await lotteryManager.WaitForDraw(purchase.DrawId);

                Console.WriteLine("Lottery session finished.");
            }

            Console.ReadKey();

            return;
        }
    }
}
