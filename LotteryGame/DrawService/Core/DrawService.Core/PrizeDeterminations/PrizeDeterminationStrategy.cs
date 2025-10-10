namespace DrawService.Core.PrizeDeterminations
{
    using DrawService.Core.Contracts;
    using DrawService.Data.Models;

    public class PrizeDeterminationStrategy : IPrizeDeterminationStrategy
    {
        private readonly IEnumerable<IPrizeStrategy> strategies;

        public PrizeDeterminationStrategy(IEnumerable<IPrizeStrategy> strategies)
        {
            this.strategies = strategies;
        }

        public IEnumerable<Prize> DeterminePrizes(Draw draw)
        {
            List<string> ticketIds = draw.PlayerTickets.SelectMany(x => x.Tickets).ToList();
            var remainingTicketIds = new List<string>(ticketIds);
            long totalRevenue = ticketIds.Count * draw.TicketPriceInCents;
            var prizes = new List<Prize>();

            foreach (var strategy in strategies)
            {
                IEnumerable<Prize> winners = strategy.Calculate(draw, remainingTicketIds, totalRevenue);
                prizes.AddRange(winners);

                remainingTicketIds.RemoveAll(id => winners.Select(x => x.TicketId).Contains(id));
            }
            
            draw.HouseProfit = totalRevenue - prizes.Sum(p => p.Amount);

            return prizes;
        }
    }
}
