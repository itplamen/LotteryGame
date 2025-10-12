namespace LotteryGame.Clients.Core.Services.Managers
{
    using LotteryGame.Clients.Core.Services.Contracts;
    using LotteryGame.Clients.Core.Services.Formatters;
    using LotteryGame.Clients.Core.Services.Models.Betting;
    using LotteryGame.Clients.Core.Services.Models.History;
    using LotteryGame.Clients.Core.Services.Models.Profile;
    using LotteryGame.Clients.Core.Wrapper.Contracts;

    public class LoggingLotteryManagerDecorator : ILotteryManager
    {
        private readonly ILotteryManager decoratee;
        private readonly IClientManager clientManager;

        public LoggingLotteryManagerDecorator(ILotteryManager decoratee, IClientManager clientManager)
        {
            this.decoratee = decoratee;
            this.clientManager = clientManager;
        }

        public async Task<ProfileResponse> GetProfile(int playerId)
        {
            clientManager.WriteLine($"Welcome to the Bede Lottery, Player {playerId}!");
            clientManager.WriteLine("");
            clientManager.WriteLine("Fetching player profile...");

            ProfileResponse resposne = await decoratee.GetProfile(playerId);

            if (resposne.Success)
            {
                clientManager.WriteLine($"* Your digital real balance is: ${resposne.RealBalance.ToString("F2")}");
                clientManager.WriteLine($"* Your digital bonus balance is: ${resposne.BonusBalance.ToString("F2")}");
                clientManager.WriteLine($"* Purchased tickets required: [{resposne.DrawOptions.MinTicketsPerPlayer} - {resposne.DrawOptions.MaxTicketsPerPlayer}]");
                clientManager.WriteLine($"* Participants required: [{resposne.DrawOptions.MinPlayersInDraw} - {resposne.DrawOptions.MaxPlayersInDraw}]");
                clientManager.WriteLine($"* Ticket Price: ${resposne.DrawOptions.TicketPrice.ToString("F2")} each");
            }
            else
            {
                clientManager.WriteLine($"ERROR: {resposne.ErrorMsg}");
            }

            return resposne;
        }

        public async Task<BettingResponse> PLayerPurchase(int playerId, int minTickets, int maxTickets)
        {
            BettingResponse response;
            bool validInput = false;

            do
            {
                clientManager.Write($"How many tickets do you want to buy, Player {playerId}?: ");

                response = await decoratee.PLayerPurchase(playerId, minTickets, maxTickets);
                validInput = response.Success;

                if (!validInput)
                {
                    clientManager.WriteLine($"ERROR: {response.ErrorMsg} Please try again ... ");
                }
            } while (!validInput);

            clientManager.WriteLine($"* Purchased Tickets: ");
            clientManager.WriteLine($"* Total cost: {MoneyFormatter.ToLabelValue(response.TotalCost)}");
            clientManager.WriteLine($"* Tickets: [{string.Join(" | ", response.TicketNumbers)}]");
            clientManager.WriteLine($"* Datetime UTC Now: [{DatetimeFormatter.Format(DateTime.UtcNow)}]");
            clientManager.WriteLine($"* Draw date: {DatetimeFormatter.Format(response.DrawDate)} [{DatetimeFormatter.TimeRemains(response.DrawDate)}]");

            return response;
        }

        public async Task<IEnumerable<BettingResponse>> CpuPurchase(int cpuStartId, int cpuCount, int numberOfTickets)
        {
            clientManager.WriteLine("Waiting for CPU players to pruchase...");

            IEnumerable<BettingResponse> responses = await decoratee.CpuPurchase(cpuStartId, cpuCount, numberOfTickets);

            if (responses.All(x => !x.Success))
            {
                clientManager.WriteLine($"ERROR: CPU players could not purchase tickets!");
            }
            else
            {
                clientManager.WriteLine($"{responses.Count(x => x.Success)} other CPU players have purchased tickets!");
            }

            return responses;
        }

        public async Task<HistoryResponse> WaitForDraw(string drawId)
        {
            clientManager.WriteLine("Waiting for the draw settlement...");

            HistoryResponse response = await decoratee.WaitForDraw(drawId);

            if (response.Success)
            {
                clientManager.WriteLine($"Draw ID: {drawId} finished! Ticket Draw Results: ");

                IEnumerable<PrizeHistory> prizes = response.Prizes
                    .OrderBy(x => x.Tier)
                    .ThenBy(x => x.PlayerId);

                foreach (var prize in prizes)
                {
                    clientManager.WriteLine($"* {prize.Tier}: Player {prize.PlayerId}, ticket number: {prize.TicketNumber}, wins: {MoneyFormatter.ToLabelValue(prize.Amount)} per winning ticket");
                }

                clientManager.WriteLine("Congratulations to the winners!");
                clientManager.WriteLine($"Draw Status: {response.DrawStatus}, House Profit: {MoneyFormatter.ToLabelValue(response.HouseProfit)}");
            }
            else
            {
                clientManager.WriteLine($"ERROR: {response.ErrorMsg}");
            }

            return response;
        }
    }
}
