namespace LotteryGame.Clients.UI.ConsoleApp
{
    using Microsoft.Extensions.Configuration;

    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Models;
    using LotteryGame.Clients.Core.Wrapper.Contracts;
    using LotteryGame.Clients.Core.Services.Formatters;

    public class ProgramManager : IProgramManager
    {
        private const int PLAYER_ID = 1;
        private const int CPU_PLAYERS_TICKETS = 2;

        private readonly IClientManager clientManager;
        private readonly ILotteryService lotteryService;
        private readonly int cpuPlayers;

        public ProgramManager(IClientManager clientManager, ILotteryService lotteryService, IConfiguration configuration)
        {
            this.clientManager = clientManager;
            this.lotteryService = lotteryService;
            cpuPlayers = int.Parse(configuration["PlayersCPU"]);
        }

        public async Task Run()
        {
            clientManager.Clear();
            clientManager.WriteLine("Welcome to the Bede Lottery, Player 1!");
            clientManager.WriteLine("");

            PlayerProfileResponse playerProfileResponse = await lotteryService.GetProfile(PLAYER_ID);

            if (!playerProfileResponse.Success) 
            {
                clientManager.WriteLine($"ERROR: {playerProfileResponse.ErrorMsg}");
                return;
            }

            clientManager.WriteLine($"* Your digital real balance is: ${playerProfileResponse.RealBalance.ToString("F2")}");
            clientManager.WriteLine($"* Your digital bonus balance is: ${playerProfileResponse.BonusBalance.ToString("F2")}");
            clientManager.WriteLine($"* Purchased tickets required: [{playerProfileResponse.DrawOptions.MinTicketsPerPlayer} - {playerProfileResponse.DrawOptions.MaxTicketsPerPlayer}]");
            clientManager.WriteLine($"* Participants required: [{playerProfileResponse.DrawOptions.MinPlayersInDraw} - {playerProfileResponse.DrawOptions.MaxPlayersInDraw}]");
            clientManager.WriteLine($"* Ticket Price: ${playerProfileResponse.DrawOptions.TicketPrice.ToString("F2")} each");

            int numberOfTickets = 0;
            bool validInput = false;

            do
            {
                clientManager.Write($"How many tickets do you want to buy, Player {PLAYER_ID}?: ");
                string line = clientManager.ReadLine();

                validInput = int.TryParse(line, out numberOfTickets) &&
                    numberOfTickets >= playerProfileResponse.DrawOptions.MinTicketsPerPlayer &&
                    numberOfTickets <= playerProfileResponse.DrawOptions.MaxTicketsPerPlayer;

                if (!validInput)
                {
                    clientManager.WriteLine("Invalid input. Please try again ... ");
                }
            } while (!validInput);

            await PurchaseTickets(numberOfTickets);

            clientManager.WriteLine("Press ENTER to exit...");
            clientManager.ReadLine();
        }

        private async Task PurchaseTickets(int numberOfTickets)
        {   
            PurchaseTicketsResponse purchaseTicketsResponse = await lotteryService.PurchaseTickets(PLAYER_ID, numberOfTickets);

            if (!purchaseTicketsResponse.Success)
            {
                clientManager.WriteLine($"ERROR: {purchaseTicketsResponse.ErrorMsg}");
                return;
            }

            clientManager.WriteLine("Purchased Tickets: ");
            clientManager.WriteLine($"Total cost: {purchaseTicketsResponse.TotalCost.ToString("F2")}");
            clientManager.WriteLine($"Tickets: [{string.Join(" | ", purchaseTicketsResponse.TicketNumbers)}]");
            clientManager.WriteLine($"Datetime UTC Now: [{DatetimeFormatter.Format(DateTime.UtcNow)}]");
            clientManager.WriteLine($"Draw date: {DatetimeFormatter.Format(purchaseTicketsResponse.DrawDate)} [{DatetimeFormatter.TimeRemains(purchaseTicketsResponse.DrawDate)}]");

            var tasks = Enumerable.Range(PLAYER_ID + 1, cpuPlayers)
                                 .Select(id => Task.Run(() => lotteryService.PurchaseTickets(id, CPU_PLAYERS_TICKETS)))
                                 .ToArray();

            clientManager.WriteLine("Waiting for CPU players...");

            PurchaseTicketsResponse[] results = await Task.WhenAll(tasks);

            if (results.All(x => !x.Success))
            {
                clientManager.WriteLine($"ERROR: CPU players could not purchase tickets!");
                return;
            }

            clientManager.WriteLine($"{results.Count(x => x.Success)} other CPU players have purchased tickets!");
        }
    }
}
