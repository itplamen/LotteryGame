namespace DrawService.Core.PrizeDeterminations
{
    using System.Security.Cryptography;

    using DrawService.Core.Contracts;
    using DrawService.Data.Models;

    public abstract class ProportionalTierPrizeStrategy : IPrizeStrategy
    {
        protected abstract decimal RevenueShare { get; }

        protected abstract decimal TicketShare { get; }
        
        protected abstract PrizeTier Tier { get; }

        public IEnumerable<Prize> Calculate(Draw draw, List<string> remainingTicketIds, long totalRevenue)
        {
            if (remainingTicketIds == null || !remainingTicketIds.Any())
            {
                return new List<Prize>();
            }

            int winnerCount = Math.Max(1, (int)Math.Floor(remainingTicketIds.Count * TicketShare));
            long prizePerTicket = (long)(totalRevenue * RevenueShare / winnerCount);

            var winners = new List<string>();
            var pool = new List<string>(remainingTicketIds);

            for (int i = 0; i < winnerCount; i++)
            {
                int index = RandomNumberGenerator.GetInt32(pool.Count);
                winners.Add(pool[index]);
                pool.RemoveAt(index);
            }

            return winners.Select(ticketId => 
                new Prize()
                {
                    DrawId = draw.Id,
                    TicketId = ticketId,
                    AmountInCents = prizePerTicket,
                    Tier = Tier
                });
        }
    }
}
